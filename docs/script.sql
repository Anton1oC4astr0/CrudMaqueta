-- ===============================================================
-- Script: reset_completo.sql
-- Proyecto: CrudMaqueta  (Sistema de Control Escolar)
-- Motor:    Oracle 19c / 21c  –  Esquema: HR_ITT
--
-- 1. DROP    – elimina tablas en orden inverso (hijas primero)
-- 2. CREATE  – recrea tablas, constraints e índices
-- 3. INSERT  – catálogos + 10 alumnos de prueba
-- 4. SELECT  – verificación final
-- ===============================================================


-- ==============================================================
-- 1. DROP (hijas primero para respetar FK)
-- ==============================================================
BEGIN EXECUTE IMMEDIATE 'DROP TABLE ALUMNOS_DISCAPACIDADES CASCADE CONSTRAINTS'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP TABLE ALUMNOS CASCADE CONSTRAINTS';                EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP TABLE DISCAPACIDADES_CAT CASCADE CONSTRAINTS';     EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP TABLE CARRERAS_CAT CASCADE CONSTRAINTS';           EXCEPTION WHEN OTHERS THEN NULL; END;
/


-- ==============================================================
-- 2. CREATE
-- ==============================================================

-- ------------------------------------------------------------
-- 2a. CARRERAS_CAT
-- ------------------------------------------------------------
CREATE TABLE CARRERAS_CAT (
    ID_CARRERA  NUMBER(3)    NOT NULL,
    NOMBRE      VARCHAR2(80) NOT NULL,
    CONSTRAINT PK_CARRERAS  PRIMARY KEY (ID_CARRERA),
    CONSTRAINT UQ_CARRERA   UNIQUE      (NOMBRE)
);

-- ------------------------------------------------------------
-- 2b. DISCAPACIDADES_CAT // catálogo de discapacidades
-- ------------------------------------------------------------
CREATE TABLE DISCAPACIDADES_CAT (
    ID_DISCAPACIDAD  NUMBER(3)    GENERATED ALWAYS AS IDENTITY START WITH 1 INCREMENT BY 1,
    NOMBRE           VARCHAR2(80) NOT NULL,
    CONSTRAINT PK_DISCAPACIDAD  PRIMARY KEY (ID_DISCAPACIDAD),
    CONSTRAINT UQ_DISCAPACIDAD  UNIQUE      (NOMBRE)
);

-- ------------------------------------------------------------
-- 2c. ALUMNOS
-- ------------------------------------------------------------
CREATE TABLE ALUMNOS (
    NUMERO_CONTROL  VARCHAR2(15)  NOT NULL,
    NOMBRE          VARCHAR2(120) NOT NULL,
    ID_CARRERA      NUMBER(3)     NOT NULL,
    CORREO          VARCHAR2(120) NOT NULL,
    FECHA_NAC       DATE          NOT NULL,
    CONSTRAINT PK_ALUMNOS   PRIMARY KEY (NUMERO_CONTROL),
    CONSTRAINT UQ_CORREO    UNIQUE      (CORREO),
    CONSTRAINT CK_CORREO    CHECK (
        REGEXP_LIKE(CORREO,'^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
    ),
    CONSTRAINT FK_CARRERA   FOREIGN KEY (ID_CARRERA)
        REFERENCES CARRERAS_CAT (ID_CARRERA)
);

CREATE INDEX IDX_ALUMNOS_CARRERA ON ALUMNOS (ID_CARRERA);

-- ------------------------------------------------------------
-- 2d. ALUMNOS_DISCAPACIDADES  // tabla intermedia para relación N:M entre alumnos y discapacidades
-- ------------------------------------------------------------
CREATE TABLE ALUMNOS_DISCAPACIDADES (
    NUMERO_CONTROL   VARCHAR2(15) NOT NULL,
    ID_DISCAPACIDAD  NUMBER(3)    NOT NULL,
    CONSTRAINT PK_ALU_DISC PRIMARY KEY (NUMERO_CONTROL, ID_DISCAPACIDAD),
    CONSTRAINT FK_AD_ALUMNO FOREIGN KEY (NUMERO_CONTROL)
        REFERENCES ALUMNOS (NUMERO_CONTROL) ON DELETE CASCADE,
    CONSTRAINT FK_AD_DISC   FOREIGN KEY (ID_DISCAPACIDAD)
        REFERENCES DISCAPACIDADES_CAT (ID_DISCAPACIDAD)
);


-- ==============================================================
-- 3. INSERT – catálogos
-- ==============================================================

-- ------------------------------------------------------------
-- 3a. CARRERAS_CAT
-- ------------------------------------------------------------
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (1, 'Sistemas Computacionales');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (2, 'Industrial');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (3, 'Electronica');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (4, 'Civil');
INSERT INTO CARRERAS_CAT (ID_CARRERA, NOMBRE) VALUES (5, 'Mecatronica');

-- ------------------------------------------------------------
-- 3b. DISCAPACIDADES_CAT
-- ------------------------------------------------------------
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Ninguna');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Visual');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Auditiva');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Motriz');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Cognitiva');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Psicosocial');
INSERT INTO DISCAPACIDADES_CAT (ID_DISCAPACIDAD, NOMBRE) VALUES ('Lenguaje');

COMMIT;

-- ------------------------------------------------------------
-- 3c. ALUMNOS
-- ------------------------------------------------------------
INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23SC001', 'Lucía Hernández Morales',         1, 'lucia.hernandez@ittoluca.edu.mx',  TO_DATE('12/05/2004','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23IN002', 'Andrés Torres Jiménez',            2, 'andres.torres@ittoluca.edu.mx',    TO_DATE('23/09/2003','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23MC003', 'Valentina Reyes Castro',           5, 'valentina.reyes@ittoluca.edu.mx',  TO_DATE('07/02/2004','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('22EL004', 'Miguel Angel Flores Gutiérrez',    3, 'miguel.flores@ittoluca.edu.mx',    TO_DATE('15/11/2002','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23CI005', 'Daniela Vargas López',             4, 'daniela.vargas@ittoluca.edu.mx',   TO_DATE('30/06/2003','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23SC006', 'Sebastián Ortiz Ramírez',          1, 'sebastian.ortiz@ittoluca.edu.mx',  TO_DATE('19/03/2004','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23IN007', 'Fernanda Aguilar Núñez',           2, 'fernanda.aguilar@ittoluca.edu.mx', TO_DATE('08/08/2003','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('22MC008', 'Rodrigo Martínez Sánchez',         5, 'rodrigo.martinez@ittoluca.edu.mx', TO_DATE('25/01/2002','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23CI009', 'Camila Díaz Peña',                 4, 'camila.diaz@ittoluca.edu.mx',      TO_DATE('14/10/2003','DD/MM/YYYY'));

INSERT INTO ALUMNOS (NUMERO_CONTROL, NOMBRE, ID_CARRERA, CORREO, FECHA_NAC)
VALUES ('23EL010', 'Javier Romero Vega',               3, 'javier.romero@ittoluca.edu.mx',    TO_DATE('03/12/2004','DD/MM/YYYY'));

-- ------------------------------------------------------------
-- 3d. ALUMNOS_DISCAPACIDADES
--
--  23SC001  Lucía        → Ninguna   (1)
--  23IN002  Andrés       → Visual    (2)
--  23MC003  Valentina    → Ninguna   (1)
--  22EL004  Miguel Angel → Auditiva  (3)
--  23CI005  Daniela      → Ninguna   (1)
--  23SC006  Sebastián    → Motriz    (4)
--  23IN007  Fernanda     → Ninguna   (1)
--  22MC008  Rodrigo      → Visual    (2) + Auditiva (3)  ← múltiple
--  23CI009  Camila       → Cognitiva (5)
--  23EL010  Javier       → Ninguna   (1)
-- ------------------------------------------------------------
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23SC001', 1);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23IN002', 2);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23MC003', 1);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('22EL004', 3);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23CI005', 1);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23SC006', 4);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23IN007', 1);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('22MC008', 2);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('22MC008', 3);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23CI009', 5);
INSERT INTO ALUMNOS_DISCAPACIDADES (NUMERO_CONTROL, ID_DISCAPACIDAD) VALUES ('23EL010', 1);

COMMIT;


-- ==============================================================
-- 4. Verificación
-- ==============================================================

-- Catálogo de carreras
SELECT * FROM CARRERAS_CAT ORDER BY ID_CARRERA;

-- Catálogo de discapacidades
SELECT * FROM DISCAPACIDADES_CAT ORDER BY ID_DISCAPACIDAD;

-- Alumnos con carrera, edad calculada y discapacidades concatenadas
SELECT
    A.NUMERO_CONTROL,
    A.NOMBRE,
    C.NOMBRE                                            AS CARRERA,
    A.CORREO,
    TO_CHAR(A.FECHA_NAC, 'DD/MM/YYYY')                 AS FECHA_NAC,
    FLOOR(MONTHS_BETWEEN(SYSDATE, A.FECHA_NAC) / 12)   AS EDAD,
    LISTAGG(D.NOMBRE, ', ')
        WITHIN GROUP (ORDER BY D.NOMBRE)                AS DISCAPACIDADES
FROM ALUMNOS A
JOIN  CARRERAS_CAT            C  ON A.ID_CARRERA      = C.ID_CARRERA
LEFT JOIN ALUMNOS_DISCAPACIDADES AD ON A.NUMERO_CONTROL = AD.NUMERO_CONTROL
LEFT JOIN DISCAPACIDADES_CAT     D  ON AD.ID_DISCAPACIDAD = D.ID_DISCAPACIDAD
GROUP BY A.NUMERO_CONTROL, A.NOMBRE, C.NOMBRE, A.CORREO, A.FECHA_NAC
ORDER BY A.NUMERO_CONTROL;