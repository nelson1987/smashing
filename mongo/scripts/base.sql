CREATE DATABASE basegeografica;

\c basegeografica;

CREATE TABLE "Customers" (
    "Id" uuid DEFAULT gen_random_uuid(),
    "Name" VARCHAR NOT NULL,
    "Email" VARCHAR NOT NULL,
    "Document" VARCHAR NOT NULL,
    PRIMARY KEY ("Id")
);