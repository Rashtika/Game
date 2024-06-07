CREATE TABLE "Inventory" (
"Id" UUID DEFAULT gen_random_uuid() PRIMARY KEY,
"Items" JSONB,
"IsActive" BOOLEAN,
"DateCreated" DATE,
"CreatedByUserId" INT,
"UpdatedByUserId" INT
);

CREATE TABLE "Champion" (
"Id" UUID DEFAULT gen_random_uuid() PRIMARY KEY,
"Name" VARCHAR(50),
"InventoryId" UUID,
"IsActive" BOOLEAN,
"DateCreated" DATE,
"CreatedByUserId" INT,
"UpdatedByUserId" INT,
CONSTRAINT FK_Champion_InventoryId FOREIGN KEY ("InventoryId") REFERENCES "Inventory" ("Id")
);

SELECT * FROM "Inventory";