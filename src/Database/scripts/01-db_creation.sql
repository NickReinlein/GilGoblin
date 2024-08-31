CREATE TABLE IF NOT EXISTS world
(
    id   INTEGER PRIMARY KEY,
    name TEXT
);

CREATE TABLE IF NOT EXISTS item
(
    id          INTEGER PRIMARY KEY,
    name        TEXT,
    description TEXT,
    icon_id     INTEGER NOT NULL,
    level       INTEGER NOT NULL,
    price_mid   INTEGER NOT NULL,
    price_low   INTEGER NOT NULL,
    stack_size  INTEGER NOT NULL,
    can_hq      BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS recipe
(
    id                         INTEGER PRIMARY KEY,
    craft_type                 INTEGER NOT NULL,
    recipe_level_table         INTEGER NOT NULL,
    target_item_id             INTEGER NOT NULL REFERENCES item (id),
    result_quantity            INTEGER NOT NULL CHECK (result_quantity > 0),
    can_hq                     BOOLEAN NOT NULL,
    can_quick_synth            BOOLEAN NOT NULL,
    item_ingredient0_target_id INTEGER NOT NULL,
    amount_ingredient0         INTEGER NOT NULL,
    item_ingredient1_target_id INTEGER NOT NULL,
    amount_ingredient1         INTEGER NOT NULL,
    item_ingredient2_target_id INTEGER NOT NULL,
    amount_ingredient2         INTEGER NOT NULL,
    item_ingredient3_target_id INTEGER NOT NULL,
    amount_ingredient3         INTEGER NOT NULL,
    item_ingredient4_target_id INTEGER NOT NULL,
    amount_ingredient4         INTEGER NOT NULL,
    item_ingredient5_target_id INTEGER NOT NULL,
    amount_ingredient5         INTEGER NOT NULL,
    item_ingredient6_target_id INTEGER NOT NULL,
    amount_ingredient6         INTEGER NOT NULL,
    item_ingredient7_target_id INTEGER NOT NULL,
    amount_ingredient7         INTEGER NOT NULL,
    item_ingredient8_target_id INTEGER NOT NULL,
    amount_ingredient8         INTEGER NOT NULL,
    item_ingredient9_target_id INTEGER NOT NULL,
    amount_ingredient9         INTEGER NOT NULL
);
CREATE INDEX idx_recipe_target_item_id ON recipe (target_item_id);

CREATE TABLE IF NOT EXISTS price_data_points
(
    id        SERIAL PRIMARY KEY,
    item_id   INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    price     NUMERIC(10, 2),
    world_id  INT, -- Nullable, used for World-level data
    dc_id     INT, -- Nullable, used for Data Center-level data
    region_id INT, -- Nullable, used for Region-level data
    timestamp BIGINT,
    UNIQUE (item_id, world_id, dc_id, region_id)
);
CREATE INDEX idx_price_data_points_item_id ON price_data_points (item_id);

CREATE TABLE IF NOT EXISTS daily_sale_velocity
(
    id              SERIAL PRIMARY KEY,
    item_id         INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_quantity  NUMERIC(12, 6),
    dc_quantity     NUMERIC(12, 6),
    region_quantity NUMERIC(12, 6),
    UNIQUE (item_id)
);
CREATE INDEX idx_daily_sale_velocity_item_id ON daily_sale_velocity (item_id);

CREATE TABLE IF NOT EXISTS min_listing
(
    id                   SERIAL PRIMARY KEY,
    item_id              INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_data_point_id  INT REFERENCES price_data_points (id),
    dc_data_point_id     INT REFERENCES price_data_points (id),
    region_data_point_id INT REFERENCES price_data_points (id),
    UNIQUE (item_id)
);
CREATE INDEX idx_min_listing_item_id ON min_listing (item_id);

CREATE TABLE IF NOT EXISTS average_sale_price
(
    id                   SERIAL PRIMARY KEY,
    item_id              INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_data_point_id  INT REFERENCES price_data_points (id),
    dc_data_point_id     INT REFERENCES price_data_points (id),
    region_data_point_id INT REFERENCES price_data_points (id),
    UNIQUE (item_id)
);
CREATE INDEX idx_average_sale_price_item_id ON average_sale_price (item_id);

CREATE TABLE IF NOT EXISTS recent_purchase
(
    id                   SERIAL PRIMARY KEY,
    item_id              INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_data_point_id  INT REFERENCES price_data_points (id),
    dc_data_point_id     INT REFERENCES price_data_points (id),
    region_data_point_id INT REFERENCES price_data_points (id),
    UNIQUE (item_id)
);
CREATE INDEX idx_recent_purchase_item_id ON recent_purchase (item_id);

CREATE TABLE IF NOT EXISTS recipecost
(
    recipe_id               INTEGER                  NOT NULL REFERENCES recipe (id) ON DELETE CASCADE,
    world_id                INTEGER                  NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    average_sale_price_cost INTEGER REFERENCES price_data_points (id),
    min_listing_price_cost  INTEGER REFERENCES price_data_points (id),
    recent_purchase_cost    INTEGER REFERENCES price_data_points (id),
    updated                 TIMESTAMP WITH TIME ZONE NOT NULL,
    PRIMARY KEY (recipe_id, world_id)
);

CREATE TABLE IF NOT EXISTS recipeprofit
(
    recipe_id                 INTEGER                  NOT NULL REFERENCES recipe (id) ON DELETE CASCADE,
    world_id                  INTEGER                  NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    average_sale_price_profit INTEGER REFERENCES price_data_points (id),
    min_listing_price_profit  INTEGER REFERENCES price_data_points (id),
    recent_purchase_price     INTEGER REFERENCES price_data_points (id),
    updated                   TIMESTAMP WITH TIME ZONE NOT NULL,
    PRIMARY KEY (recipe_id, world_id)
);
