create table ItemInfo
(
    ID          INTEGER not null
        constraint PK_ItemInfo
            primary key autoincrement,
    Name        TEXT,
    Description TEXT,
    IconID      INTEGER not null,
    Level       INTEGER not null,
    VendorPrice INTEGER not null,
    StackSize   INTEGER not null,
    CanBeHq     INTEGER not null
);

create table Price
(
    ItemID                INTEGER not null,
    WorldID               INTEGER not null,
    LastUploadTime        INTEGER not null,
    AverageListingPrice   REAL    not null,
    AverageListingPriceNQ REAL    not null,
    AverageListingPriceHQ REAL    not null,
    AverageSold           REAL    not null,
    AverageSoldNQ         REAL    not null,
    AverageSoldHQ         REAL    not null,
    constraint PK_Price
        primary key (ItemID, WorldID)
);

create table Recipe
(
    ID                      INTEGER not null
        constraint PK_Recipe
            primary key autoincrement,
    TargetItemID            INTEGER not null,
    ResultQuantity          INTEGER not null,
    CanHq                   INTEGER not null,
    CanQuickSynth           INTEGER not null,
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
    ItemIngredient0TargetID INTEGER not null,
    ItemIngredient1TargetID INTEGER not null,
    ItemIngredient2TargetID INTEGER not null,
    ItemIngredient3TargetID INTEGER not null,
    ItemIngredient4TargetID INTEGER not null,
    ItemIngredient5TargetID INTEGER not null,
    ItemIngredient6TargetID INTEGER not null,
    ItemIngredient7TargetID INTEGER not null,
    ItemIngredient8TargetID INTEGER not null,
    ItemIngredient9TargetID INTEGER not null
);

create table RecipeCost
(
    RecipeID INT            not null,
    WorldID  INT            not null,
    Cost     INT            not null,
    Created  DATETIMEOFFSET not null,
    Updated  DATETIMEOFFSET not null,
    primary key (RecipeID, WorldID)
);


