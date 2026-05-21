--===============================================================
-- Proyecto:    CrudMaqueta  (Sistema de Control Escolar)
-- Autor:       Equipo verde
-- Motor:       Oracle 19c / 21c
-- Esquema:     HR_ITT
--
-- Este script crea la base de datos del sistema:
--   * Catálogos: CARRERAS_CAT y DISCAPACIDADES_CAT
--   * Tabla principal: ALUMNOS con sus llaves foráneas y constraints
--   * Datos semilla de los catálogos
--
-- Orden de ejecución:
--   1) Limpia objetos previos (DROP)
--   2) Crea catálogos
--   3) Inserta datos de catálogo
--   4) Crea ALUMNOS y restricciones
--   5) Inserta datos de prueba
--===============================================================

-- ---------------------------------------------------------------
-- 1. Eliminar objetos previos (modo seguro)
-- ---------------------------------------------------------------
BEGIN EXECUTE IMMEDIATE 'DROP TABLE ALUMNOS CASCADE CONSTRAINTS'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP TABLE CARRERAS_CAT CASCADE CONSTRAINTS'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP TABLE DISCAPACIDADES_CAT CASCADE CONSTRAINTS'; EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- ---------------------------------------------------------------
-- 2. Catálogo de Carreras
-- ---------------------------------------------------------------
CREATE TABLE CARRERAS_CAT (
    ID_CARRERA   NUMBER(3)      NOT NULL,
    NOMBRE       VARCHAR2(80)   NOT NULL,
    CONSTRAINT PK_CARRERAS
        PRIMARY KEY(ID_CARRERA),
    CONSTRAINT UQ_CARRERA
        UNIQUE(NOMBRE)
);

-- ---------------------------------------------------------------
-- 3. Catálogo de Discapacidades
-- ---------------------------------------------------------------
CREATE TABLE DISCAPACIDADES_CAT (
    ID_DISCAPACIDAD   NUMBER(3)      NOT NULL,
    NOMBRE            VARCHAR2(80)   NOT NULL,
    CONSTRAINT PK_DISCAPACIDAD
        PRIMARY KEY(ID_DISCAPACIDAD),
    CONSTRAINT UQ_DISCAPACIDAD
        UNIQUE(NOMBRE));

-- ---------------------------------------------------------------
-- 4. Datos semilla del catálogo de Carreras
-- ---------------------------------------------------------------
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (1, 'Sistemas Computacionales');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (2, 'Industrial');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (3, 'Electronica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (4, 'Civil');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (5, 'Mecatronica');
COMMIT;

-- ---------------------------------------------------------------
-- 5. Datos semilla del catálogo de Discapacidades
--    "Ninguna" es la opción por defecto.
--    "Otra" permite registrar valores no previstos
--    (el sistema almacena el texto libre en ALUMNOS.DISCAPACIDAD).
-- ---------------------------------------------------------------
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (1, 'Ninguna');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (2, 'Visual');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (3, 'Auditiva');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (4, 'Motriz');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (5, 'Cognitiva');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (6, 'Psicosocial');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (7, 'Lenguaje');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES (8, 'Otra');
COMMIT;

-- ---------------------------------------------------------------
-- 6. Tabla principal ALUMNOS
--    * CARRERA es texto, validado contra el catálogo CARRERAS_CAT
--      con la FK_ALUMNOS_CARRERA (carrera DEBE existir como NOMBRE
--      en el catálogo).
--    * DISCAPACIDAD también es texto: para los valores del catálogo
--      se valida con un trigger ligero (ver fin del script). Si el
--      usuario elige "Otra" en la interfaz, se permite cualquier
--      cadena.
-- ---------------------------------------------------------------
CREATE TABLE ALUMNOS(
    NUMERO_CONTROL VARCHAR2(15) NOT NULL,
    NOMBRE VARCHAR2(120)
        NOT NULL,
    CORREO VARCHAR2(120)
        NOT NULL,
    FECHA_NAC DATE
        NOT NULL,
    ID_CARRERA NUMBER
        NOT NULL,
    ID_DISCAPACIDAD NUMBER
        DEFAULT NULL,
    CONSTRAINT PK_ALUMNOS
        PRIMARY KEY(NUMERO_CONTROL),
    CONSTRAINT UQ_CORREO
        UNIQUE(CORREO),
    CONSTRAINT CK_CORREO
        CHECK(
            REGEXP_LIKE(
                CORREO,
                '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'
            )
        ),
    CONSTRAINT FK_CARRERA
        FOREIGN KEY(ID_CARRERA)
        REFERENCES CARRERAS_CAT(ID_CARRERA),
    CONSTRAINT FK_DISCAPACIDAD
        FOREIGN KEY(ID_DISCAPACIDAD)
        REFERENCES DISCAPACIDADES_CAT(ID_DISCAPACIDAD));

-- Índices auxiliares para acelerar búsqueda por carrera
CREATE INDEX IDX_ALUMNOS_CARRERA ON ALUMNOS(CARRERA);


-- ---------------------------------------------------------------
-- 7. Trigger de validación de DISCAPACIDAD
--    Permite los valores del catálogo o, si la cadena no aparece,
--    se considera "Otra" (texto libre).
-- ---------------------------------------------------------------
CREATE OR REPLACE TRIGGER TRG_ALUMNOS_DISCAPACIDAD
BEFORE INSERT OR UPDATE OF DISCAPACIDAD ON ALUMNOS
FOR EACH ROW
DECLARE
    v_existe NUMBER;
BEGIN
    IF :NEW.DISCAPACIDAD IS NULL OR LENGTH(TRIM(:NEW.DISCAPACIDAD)) = 0 THEN
        :NEW.DISCAPACIDAD := 'Ninguna';
    END IF;

    SELECT COUNT(*) INTO v_existe
      FROM DISCAPACIDADES_CAT
     WHERE NOMBRE = :NEW.DISCAPACIDAD;

    -- Si no está en el catálogo, asumimos que es el texto libre que
    -- el usuario eligió cuando seleccionó "Otra" → se permite tal cual.
    IF v_existe = 0 THEN
        NULL; -- valor libre aceptado
    END IF;
END;
/

-- ---------------------------------------------------------------
-- 8. Datos de prueba
-- ---------------------------------------------------------------
INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD)
VALUES ('20230101', 'Ana Lopez García', 'Sistemas Computacionales',
        'ana.lopez@ittoluca.edu.mx', TO_DATE('15/03/2003','DD/MM/YYYY'), 22, 'Ninguna');

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD)
VALUES ('20230102', 'Carlos Perez Hernandez', 'Industrial',
        'carlos.perez@ittoluca.edu.mx', TO_DATE('07/07/2002','DD/MM/YYYY'), 23, 'Visual');

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD)
VALUES ('20230103', 'Maria Sanchez Ruiz', 'Mecatronica',
        'maria.sanchez@ittoluca.edu.mx', TO_DATE('22/11/2003','DD/MM/YYYY'), 22, 'Ninguna');

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD)
VALUES ('20230104', 'Diego Ramirez Soto', 'Electronica',
        'diego.ramirez@ittoluca.edu.mx', TO_DATE('04/01/2003','DD/MM/YYYY'), 23, 'Auditiva');

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, CARRERA, CORREO, FECHA_NAC, EDAD, DISCAPACIDAD)
VALUES ('20230105', 'Sofia Mendez Cruz', 'Civil',
        'sofia.mendez@ittoluca.edu.mx', TO_DATE('18/09/2003','DD/MM/YYYY'), 22, 'Dislexia'); -- valor libre "Otra"
COMMIT;

-- ---------------------------------------------------------------
-- 9. Consultas de verificación
-- ---------------------------------------------------------------
SELECT * FROM CARRERAS_CAT       ORDER BY ID_CARRERA;
SELECT * FROM DISCAPACIDADES_CAT ORDER BY ID_DISCAPACIDAD;
SELECT NUMERO_CONTROL, NOMBRE, CARRERA, DISCAPACIDAD
  FROM ALUMNOS
 ORDER BY NUMERO_CONTROL;
