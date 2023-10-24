create table if not exists item
(
    id          INTEGER PRIMARY KEY,
    name        TEXT,
    description TEXT,
    iconid      INTEGER not null,
    level       INTEGER not null,
    pricemid    INTEGER not null,
    pricelow    INTEGER not null,
    stacksize   INTEGER not null,
    canbehq     BOOLEAN not null
);

create table if not exists price
(
    itemid                INTEGER not null,
    worldid               INTEGER not null,
    lastuploadtime        BIGINT not null,
    averagelistingprice   REAL    not null,
    averagelistingpricenq REAL    not null,
    averagelistingpricehq REAL    not null,
    averagesold           REAL    not null,
    averagesoldnq         REAL    not null,
    averagesoldhq         REAL    not null,
    constraint PK_price
        primary key (itemid, worldid)
);

create table if not exists recipe
(
    id                      INTEGER PRIMARY KEY,
    crafttype               INTEGER not null,
    recipeleveltable        INTEGER not null,
    targetitemid            INTEGER not null,
    resultquantity          INTEGER not null,
    canhq                   BOOLEAN not null,
    canquicksynth           BOOLEAN not null,
    itemingredient0targetid INTEGER not null,
    amountingredient0       INTEGER not null,
    itemingredient1targetid INTEGER not null,
    amountingredient1       INTEGER not null,
    itemingredient2targetid INTEGER not null,
    amountingredient2       INTEGER not null,
    itemingredient3targetid INTEGER not null,
    amountingredient3       INTEGER not null,
    itemingredient4targetid INTEGER not null,
    amountingredient4       INTEGER not null,
    itemingredient5targetid INTEGER not null,
    amountingredient5       INTEGER not null,
    itemingredient6targetid INTEGER not null,
    amountingredient6       INTEGER not null,
    itemingredient7targetid INTEGER not null,
    amountingredient7       INTEGER not null,
    itemingredient8targetid INTEGER not null,
    amountingredient8       INTEGER not null,
    itemingredient9targetid INTEGER not null,
    amountingredient9       INTEGER not null
);

create table if not exists recipecost
(
    recipeid INTEGER            not null,
    worldid  INTEGER            not null,
    cost     INTEGER            not null,
    created  TIMESTAMP WITH TIME ZONE NOT NULL,
    updated  TIMESTAMP WITH TIME ZONE NOT NULL,
    primary key (recipeid, worldid)
);