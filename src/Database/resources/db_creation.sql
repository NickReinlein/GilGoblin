create table if not exists item
(
    Id          INTEGER PRIMARY KEY,
    Name        TEXT,
    Description TEXT,
    IconId      INTEGER not null,
    Level       INTEGER not null,
    VendorPrice INTEGER not null,
    StackSize   INTEGER not null,
    CanBeHq     BOOLEAN not null
);

create table if not exists price
(
    ItemId                INTEGER not null,
    WorldId               INTEGER not null,
    LastUploadTime        BIGINT not null,
    AverageListingPrice   REAL    not null,
    AverageListingPriceNQ REAL    not null,
    AverageListingPriceHQ REAL    not null,
    AverageSold           REAL    not null,
    AverageSoldNQ         REAL    not null,
    AverageSoldHQ         REAL    not null,
    constraint PK_Price
        primary key (ItemId, WorldId)
);

create table if not exists recipe
(
    Id                      INTEGER PRIMARY KEY,
    TargetItemId            INTEGER not null,
    ResultQuantity          INTEGER not null,
    CanHq                   BOOLEAN not null,
    CanQuickSynth           BOOLEAN not null,
    AmountIngredient0       INTEGER not null,
    AmountIngredient1       INTEGER not null,
    AmountIngredient2       INTEGER not null,
    AmountIngredient3       INTEGER not null,
    AmountIngredient4       INTEGER not null,
    AmountIngredient5       INTEGER not null,
    AmountIngredient6       INTEGER not null,
    AmountIngredient7       INTEGER not null,
    AmountIngredient8       INTEGER not null,
    AmountIngredient9       INTEGER not null,
    ItemIngredient0TargetId INTEGER not null,
    ItemIngredient1TargetId INTEGER not null,
    ItemIngredient2TargetId INTEGER not null,
    ItemIngredient3TargetId INTEGER not null,
    ItemIngredient4TargetId INTEGER not null,
    ItemIngredient5TargetId INTEGER not null,
    ItemIngredient6TargetId INTEGER not null,
    ItemIngredient7TargetId INTEGER not null,
    ItemIngredient8TargetId INTEGER not null,
    ItemIngredient9TargetId INTEGER not null
);

create table if not exists recipe_cost
(
    RecipeId INTEGER            not null,
    WorldId  INTEGER            not null,
    Cost     INTEGER            not null,
    Created  TIMESTAMP WITH TIME ZONE NOT NULL,
    Updated  TIMESTAMP WITH TIME ZONE NOT NULL,
    primary key (RecipeId, WorldId)
);