-- ---------------------------------------------------------------
-- 1. Eliminar objetos previos, se usa DELETE directo
-- ---------------------------------------------------------------
DROP TABLE ALUMNOS CASCADE CONSTRAINTS;
DROP TABLE CARRERAS_CAT CASCADE CONSTRAINTS;
DROP TABLE DISCAPACIDADES_CAT CASCADE CONSTRAINTS;

-- ---------------------------------------------------------------
-- 2. Catálogo de Carreras
-- ---------------------------------------------------------------
CREATE TABLE CARRERAS_CAT (
    ID_CARRERA   NUMBER(3)      NOT NULL,
    NOMBRE       VARCHAR2(80)   NOT NULL,
    CONSTRAINT PK_CARRERAS_CAT PRIMARY KEY (ID_CARRERA),
    CONSTRAINT UQ_CARRERAS_CAT_NOMBRE UNIQUE (NOMBRE)
);

-- ---------------------------------------------------------------
-- 3. Catálogo de Discapacidades
--    ESTATUS: 'CATALOGO' para las predefinidas, 'OTRA' para las agregadas por usuarios
-- ---------------------------------------------------------------
CREATE TABLE DISCAPACIDADES_CAT (
    ID_DISCAPACIDAD   NUMBER(3)      NOT NULL,
    NOMBRE            VARCHAR2(80)   NOT NULL,
    ESTATUS           VARCHAR2(20)   DEFAULT 'CATALOGO' NOT NULL,
    CONSTRAINT PK_DISCAPACIDADES_CAT PRIMARY KEY (ID_DISCAPACIDAD),
    CONSTRAINT UQ_DISCAPACIDADES_CAT_NOMBRE UNIQUE (NOMBRE),
    CONSTRAINT CK_DISCAPACIDADES_ESTATUS CHECK (ESTATUS IN ('CATALOGO', 'OTRA'))
);

-- ---------------------------------------------------------------
-- 4. Datos del catálogo de Carreras
-- ---------------------------------------------------------------
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (1, 'Electronica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (2, 'Electromecanica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (3, 'Gestion Empresarial');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (4, 'Industrial');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (5, 'Logistica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (6, 'Mecatronica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (7, 'Quimica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (8, 'Sistemas Computacionales');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (9, 'TICs');
COMMIT;

-- ---------------------------------------------------------------
-- 5. Datos del catálogo de Discapacidades
--    Las discapacidades agregadas por usuarios tendrán ESTATUS='OTRA'
-- ---------------------------------------------------------------
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (1, 'Visual', 'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (2, 'Auditiva', 'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (3, 'Motriz', 'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (4, 'Cognitiva', 'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (5, 'Psicosocial', 'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (6, 'Lenguaje', 'CATALOGO');
COMMIT;

-- ---------------------------------------------------------------
-- 6. Tabla ALUMNOS 
-- ---------------------------------------------------------------
CREATE TABLE ALUMNOS (
    NUMERO_CONTROL   VARCHAR2(15)   NOT NULL,
    NOMBRE           VARCHAR2(120)  NOT NULL,
    ID_CARRERA       NUMBER(3)      NOT NULL,
    CORREO           VARCHAR2(120)  NOT NULL,
    FECHA_NAC        DATE           NOT NULL,
    EDAD             NUMBER(3)      NOT NULL, 
    ID_DISCAPACIDAD  NUMBER(3)      NULL,     -- NULL significa "Ninguna"
    CONSTRAINT PK_ALUMNOS                PRIMARY KEY (NUMERO_CONTROL),
    CONSTRAINT UQ_ALUMNOS_CORREO         UNIQUE (CORREO),
    CONSTRAINT FK_ALUMNOS_CARRERA        FOREIGN KEY (ID_CARRERA)
        REFERENCES CARRERAS_CAT(ID_CARRERA),
    CONSTRAINT FK_ALUMNOS_DISCAPACIDAD   FOREIGN KEY (ID_DISCAPACIDAD)
        REFERENCES DISCAPACIDADES_CAT(ID_DISCAPACIDAD)
);

-- ---------------------------------------------------------------
-- 7. Secuencia simplificada para IDs automáticos
-- ---------------------------------------------------------------
CREATE SEQUENCE SEQ_DISCAPACIDADES_CAT 
    START WITH 7 
    INCREMENT BY 1;

-- ---------------------------------------------------------------
-- 8. Datos de prueba (Cambiado el 1 por NULL para los que no tienen discapacidad)
-- ---------------------------------------------------------------
INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230101', 'Ana Lopez García', 1, 'ana.lopez@ittoluca.edu.mx', TO_DATE('15/03/2003','DD/MM/YYYY'), 22, NULL);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230102', 'Carlos Perez Hernandez', 2, 'carlos.perez@ittoluca.edu.mx', TO_DATE('07/07/2002','DD/MM/YYYY'), 23, 2);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230103', 'Maria Sanchez Ruiz', 5, 'maria.sanchez@ittoluca.edu.mx', TO_DATE('22/11/2003','DD/MM/YYYY'), 22, NULL);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230104', 'Diego Ramirez Soto', 3, 'diego.ramirez@ittoluca.edu.mx', TO_DATE('04/01/2003','DD/MM/YYYY'), 23, 3);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230105', 'Sofia Mendez Cruz', 4, 'sofia.mendez@ittoluca.edu.mx', TO_DATE('18/09/2003','DD/MM/YYYY'), 22, 5);
COMMIT;

-- ---------------------------------------------------------------
-- 9. Consultas de verificación
-- ---------------------------------------------------------------

-- HEADING no cambia el nombre de columna, solo la "etiqueta" visual
-- FORMAT A(número) define el ancho visual de las columnas de texto
COLUMN NUMERO_CONTROL HEADING 'No. Control'  FORMAT A12;
COLUMN NOMBRE         HEADING 'Alumno'       FORMAT A30;
COLUMN CARRERA        HEADING 'Carrera'      FORMAT A25;
COLUMN DISCAPACIDAD   HEADING 'Discapacidad' FORMAT A20;

SET PAGESIZE 50; -- Cuántas filas muestra antes de repetir los encabezados
SET LINESIZE 150; -- Ancho total de la fila en pantalla (evita que se corte)

SELECT * FROM CARRERAS_CAT       ORDER BY ID_CARRERA;
SELECT * FROM DISCAPACIDADES_CAT ORDER BY ID_DISCAPACIDAD;

SELECT a.NUMERO_CONTROL, a.NOMBRE, c.NOMBRE AS CARRERA, d.NOMBRE AS DISCAPACIDAD
  FROM ALUMNOS a
  LEFT JOIN CARRERAS_CAT c ON a.ID_CARRERA = c.ID_CARRERA
  LEFT JOIN DISCAPACIDADES_CAT d ON a.ID_DISCAPACIDAD = d.ID_DISCAPACIDAD
 ORDER BY a.NUMERO_CONTROL;