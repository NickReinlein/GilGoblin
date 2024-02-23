create table if not exists item
(
    id          INTEGER PRIMARY KEY,
    name        TEXT,
    description TEXT,
    iconid      INTEGER NOT NULL,
    level       INTEGER NOT NULL,
    pricemid    INTEGER NOT NULL,
    pricelow    INTEGER NOT NULL,
    stacksize   INTEGER NOT NULL,
    canhq       BOOLEAN NOT NULL
);

create table if not exists price
(
    itemid                INTEGER NOT NULL CHECK (itemid > 0),
    worldid               INTEGER NOT NULL CHECK (worldid > 0),
    lastuploadtime        BIGINT  NOT NULL,
    averagelistingprice   REAL    NOT NULL,
    averagelistingpricenq REAL    NOT NULL,
    averagelistingpricehq REAL    NOT NULL,
    averagesold           REAL    NOT NULL,
    averagesoldnq         REAL    NOT NULL,
    averagesoldhq         REAL    NOT NULL,
    constraint PK_price primary key (itemid, worldid)
);

create table if not exists recipe
(
    id                      INTEGER PRIMARY KEY,
    crafttype               INTEGER NOT NULL,
    recipeleveltable        INTEGER NOT NULL,
    targetitemid            INTEGER NOT NULL CHECK (targetitemid > 0),
    resultquantity          INTEGER NOT NULL CHECK (resultquantity > 0),
    canhq                   BOOLEAN NOT NULL,
    canquicksynth           BOOLEAN NOT NULL,
    itemingredient0targetid INTEGER NOT NULL,
    amountingredient0       INTEGER NOT NULL,
    itemingredient1targetid INTEGER NOT NULL,
    amountingredient1       INTEGER NOT NULL,
    itemingredient2targetid INTEGER NOT NULL,
    amountingredient2       INTEGER NOT NULL,
    itemingredient3targetid INTEGER NOT NULL,
    amountingredient3       INTEGER NOT NULL,
    itemingredient4targetid INTEGER NOT NULL,
    amountingredient4       INTEGER NOT NULL,
    itemingredient5targetid INTEGER NOT NULL,
    amountingredient5       INTEGER NOT NULL,
    itemingredient6targetid INTEGER NOT NULL,
    amountingredient6       INTEGER NOT NULL,
    itemingredient7targetid INTEGER NOT NULL,
    amountingredient7       INTEGER NOT NULL,
    itemingredient8targetid INTEGER NOT NULL,
    amountingredient8       INTEGER NOT NULL,
    itemingredient9targetid INTEGER NOT NULL,
    amountingredient9       INTEGER NOT NULL
);

create table if not exists recipecost
(
    recipeid INTEGER                  NOT NULL CHECK (recipeid > 0),
    worldid  INTEGER                  NOT NULL CHECK (worldid > 0),
    cost     INTEGER                  NOT NULL CHECK (cost > 0),
    updated  TIMESTAMP WITH TIME ZONE NOT NULL,
    primary key (recipeid, worldid)
);

create table if not exists recipeprofit
(
    recipeid         INTEGER                  NOT NULL CHECK (recipeid > 0),
    worldid          INTEGER                  NOT NULL CHECK (worldid > 0),
    profitvssold     INTEGER                  NOT NULL,
    profitvslistings INTEGER                  NOT NULL,
    updated          TIMESTAMP WITH TIME ZONE NOT NULL,
    primary key (recipeid, worldid)
);

create table if not exists world
(
    id   INTEGER PRIMARY KEY,
    name TEXT
);