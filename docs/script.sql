-- ---------------------------------------------------------------
-- 1. Eliminar objetos previos
-- ---------------------------------------------------------------
DROP SEQUENCE SEQ_DISCAPACIDADES_CAT;
DROP TABLE ALUMNOS CASCADE CONSTRAINTS;
DROP TABLE CARRERAS_CAT CASCADE CONSTRAINTS;
DROP TABLE DISCAPACIDADES_CAT CASCADE CONSTRAINTS;

-- ---------------------------------------------------------------
-- 2. Catálogo de Carreras
-- ---------------------------------------------------------------
CREATE TABLE CARRERAS_CAT (
    ID_CARRERA   NUMBER(3)      NOT NULL,
    NOMBRE       VARCHAR2(80)   NOT NULL,
    CONSTRAINT PK_CARRERAS_CAT        PRIMARY KEY (ID_CARRERA),
    CONSTRAINT UQ_CARRERAS_CAT_NOMBRE UNIQUE (NOMBRE)
);

-- ---------------------------------------------------------------
-- 3. Catálogo de Discapacidades
--    ESTATUS: 'CATALOGO' para las predefinidas
--             'OTRA'     para las agregadas por el usuario
-- ---------------------------------------------------------------
CREATE TABLE DISCAPACIDADES_CAT (
    ID_DISCAPACIDAD   NUMBER(3)      NOT NULL,
    NOMBRE            VARCHAR2(80)   NOT NULL,
    ESTATUS           VARCHAR2(20)   DEFAULT 'CATALOGO' NOT NULL,
    CONSTRAINT PK_DISCAPACIDADES_CAT        PRIMARY KEY (ID_DISCAPACIDAD),
    CONSTRAINT UQ_DISCAPACIDADES_CAT_NOMBRE UNIQUE (NOMBRE),
    CONSTRAINT CK_DISCAPACIDADES_ESTATUS    CHECK (ESTATUS IN ('CATALOGO', 'OTRA'))
);

-- ---------------------------------------------------------------
-- 4. Tabla ALUMNOS
--    ID_DISCAPACIDAD NULL significa "Ninguna discapacidad"
-- ---------------------------------------------------------------
CREATE TABLE ALUMNOS (
    NUMERO_CONTROL   VARCHAR2(15)   NOT NULL,
    NOMBRE           VARCHAR2(120)  NOT NULL,
    ID_CARRERA       NUMBER(3)      NOT NULL,
    CORREO           VARCHAR2(120)  NOT NULL,
    FECHA_NAC        DATE           NOT NULL,
    EDAD             NUMBER(3)      NOT NULL,
    ID_DISCAPACIDAD  NUMBER(3)      NULL,
    CONSTRAINT PK_ALUMNOS              PRIMARY KEY (NUMERO_CONTROL),
    CONSTRAINT UQ_ALUMNOS_CORREO       UNIQUE (CORREO),
    CONSTRAINT FK_ALUMNOS_CARRERA      FOREIGN KEY (ID_CARRERA)
        REFERENCES CARRERAS_CAT(ID_CARRERA),
    CONSTRAINT FK_ALUMNOS_DISCAPACIDAD FOREIGN KEY (ID_DISCAPACIDAD)
        REFERENCES DISCAPACIDADES_CAT(ID_DISCAPACIDAD)
);

-- ---------------------------------------------------------------
-- 5. Secuencia para IDs automáticos de discapacidades nuevas
--    Empieza en 7 porque el catálogo base ocupa del 1 al 6
-- ---------------------------------------------------------------
CREATE SEQUENCE SEQ_DISCAPACIDADES_CAT
    START WITH 7
    INCREMENT BY 1;

-- ---------------------------------------------------------------
-- 6. Datos del catálogo de Carreras
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
-- 7. Datos del catálogo de Discapacidades (predefinidas)
-- ---------------------------------------------------------------
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (1, 'Visual',      'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (2, 'Auditiva',    'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (3, 'Motriz',      'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (4, 'Cognitiva',   'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (5, 'Psicosocial', 'CATALOGO');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS) VALUES (6, 'Lenguaje',    'CATALOGO');
COMMIT;

-- ---------------------------------------------------------------
-- 8. Ejemplo de uso de la secuencia
--    Así se insertaría una discapacidad nueva agregada por el usuario
-- ---------------------------------------------------------------
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE, ESTATUS)
VALUES (SEQ_DISCAPACIDADES_CAT.NEXTVAL, 'Dislexia', 'OTRA');
COMMIT;

-- ---------------------------------------------------------------
-- 9. Datos de prueba de Alumnos
--    ID_DISCAPACIDAD NULL = sin discapacidad registrada
-- ---------------------------------------------------------------
INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230101', 'Ana Lopez Garcia',      1, 'ana.lopez@ittoluca.edu.mx',    TO_DATE('15/03/2003','DD/MM/YYYY'), 22, NULL);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230102', 'Carlos Perez Hernandez', 2, 'carlos.perez@ittoluca.edu.mx', TO_DATE('07/07/2002','DD/MM/YYYY'), 23, 2);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230103', 'Maria Sanchez Ruiz',    5, 'maria.sanchez@ittoluca.edu.mx', TO_DATE('22/11/2003','DD/MM/YYYY'), 22, NULL);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230104', 'Diego Ramirez Soto',    3, 'diego.ramirez@ittoluca.edu.mx', TO_DATE('04/01/2003','DD/MM/YYYY'), 23, 3);

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC, EDAD, ID_DISCAPACIDAD)
VALUES ('20230105', 'Sofia Mendez Cruz',     4, 'sofia.mendez@ittoluca.edu.mx',  TO_DATE('18/09/2003','DD/MM/YYYY'), 22, 5);
COMMIT;

-- ---------------------------------------------------------------
-- 10. Consultas de verificación
-- ---------------------------------------------------------------

SET PAGESIZE 50;
SET LINESIZE 150;

-- Carreras
COLUMN ID_CARRERA FORMAT 99;
COLUMN NOMBRE     FORMAT A30;
SELECT * FROM CARRERAS_CAT
ORDER BY ID_CARRERA;

-- Discapacidades
COLUMN ID_DISCAPACIDAD FORMAT 99;
COLUMN NOMBRE          FORMAT A25;
COLUMN ESTATUS         FORMAT A10;
SELECT * FROM DISCAPACIDADES_CAT
ORDER BY ID_DISCAPACIDAD;

-- Alumnos con sus IDs y nombres de carrera y discapacidad
COLUMN "NO. CONTROL"     FORMAT A12;
COLUMN "NOMBRE COMPLETO" FORMAT A25;
COLUMN ID_CAR            FORMAT 99;
COLUMN CARRERA           FORMAT A20;
COLUMN CORREO            FORMAT A30;
COLUMN "FECHA NAC."      FORMAT A12;
COLUMN ID_DIS            FORMAT 99;
COLUMN DISCAPACIDAD      FORMAT A15;

SELECT
    A.NUMERO_CONTROL             AS "NO. CONTROL",
    A.NOMBRE                     AS "NOMBRE COMPLETO",
    A.ID_CARRERA                 AS ID_CAR,
    C.NOMBRE                     AS CARRERA,
    A.CORREO,
    A.FECHA_NAC                  AS "FECHA NAC.",
    A.ID_DISCAPACIDAD            AS ID_DIS,
    NVL(D.NOMBRE, 'Ninguna')     AS DISCAPACIDAD
FROM ALUMNOS A
JOIN CARRERAS_CAT C
    ON A.ID_CARRERA = C.ID_CARRERA
LEFT JOIN DISCAPACIDADES_CAT D
    ON A.ID_DISCAPACIDAD = D.ID_DISCAPACIDAD
ORDER BY A.NUMERO_CONTROL;