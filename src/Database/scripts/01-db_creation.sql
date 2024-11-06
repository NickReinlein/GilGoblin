CREATE TABLE IF NOT EXISTS world
(
    id   SERIAL PRIMARY KEY,
    name TEXT
);

CREATE TABLE IF NOT EXISTS item
(
    id          SERIAL PRIMARY KEY,
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
    id                         SERIAL PRIMARY KEY,
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

CREATE TABLE IF NOT EXISTS price_data
(
    id         SERIAL PRIMARY KEY,
    price_type VARCHAR(20)    NOT NULL,
    price      NUMERIC(12, 2) NOT NULL,
    world_id   INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    timestamp  BIGINT
);

CREATE TABLE IF NOT EXISTS world_upload_times
(
    id        SERIAL PRIMARY KEY,
    item_id   INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_id  INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq     BOOLEAN NOT NULL,
    timestamp BIGINT  NOT NULL
);
CREATE INDEX idx_world_upload_times_item_id_and_world_id_and_is_hq ON world_upload_times (item_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS daily_sale_velocity
(
    id              SERIAL PRIMARY KEY,
    item_id         INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_id        INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq           BOOLEAN NOT NULL,
    world_quantity  NUMERIC(12, 2),
    dc_quantity     NUMERIC(12, 2),
    region_quantity NUMERIC(12, 2)
);
CREATE INDEX idx_daily_sale_velocity_item_id_and_world_id_and_is_hq ON daily_sale_velocity (item_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS average_sale_price
(
    id                   SERIAL PRIMARY KEY,
    item_id              INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_id             INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq                BOOLEAN NOT NULL,
    world_data_point_id  INT REFERENCES price_data (id) ON DELETE CASCADE,
    dc_data_point_id     INT REFERENCES price_data (id) ON DELETE CASCADE,
    region_data_point_id INT REFERENCES price_data (id) ON DELETE CASCADE
);
CREATE INDEX idx_average_sale_price_item_id_and_world_id_and_is_hq ON average_sale_price (item_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS min_listing
(
    id                   SERIAL PRIMARY KEY,
    item_id              INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_id             INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq                BOOLEAN NOT NULL,
    world_data_point_id  INT REFERENCES price_data (id) ON DELETE CASCADE,
    dc_data_point_id     INT REFERENCES price_data (id) ON DELETE CASCADE,
    region_data_point_id INT REFERENCES price_data (id) ON DELETE CASCADE
);
CREATE INDEX idx_min_listing_item_id_and_world_id_and_is_hq ON min_listing (item_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS recent_purchase
(
    id                   SERIAL PRIMARY KEY,
    item_id              INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_id             INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq                BOOLEAN NOT NULL,
    world_data_point_id  INT REFERENCES price_data (id) ON DELETE CASCADE,
    dc_data_point_id     INT REFERENCES price_data (id) ON DELETE CASCADE,
    region_data_point_id INT REFERENCES price_data (id) ON DELETE CASCADE
);
CREATE INDEX idx_recent_purchase_item_id_and_world_id_and_is_hq ON recent_purchase (item_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS recipe_cost
(
    id                      SERIAL PRIMARY KEY,
    recipe_id               INTEGER NOT NULL REFERENCES recipe (id) ON DELETE CASCADE,
    world_id                INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq                   BOOLEAN NOT NULL,
    amount                  INTEGER NOT NULL,
    last_updated            TIMESTAMP WITH TIME ZONE NOT NULL,
    UNIQUE (recipe_id, world_id, is_hq)
);
CREATE INDEX idx_recipe_cost_recipe_and_world_id_and_is_hq ON recipe_cost (recipe_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS recipe_profit
(
    id                     SERIAL PRIMARY KEY,
    recipe_id              INTEGER NOT NULL REFERENCES recipe (id) ON DELETE CASCADE,
    world_id               INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq                  BOOLEAN NOT NULL,
    amount                 INTEGER NOT NULL,
    last_updated           TIMESTAMP WITH TIME ZONE NOT NULL,
    UNIQUE (recipe_id, world_id, is_hq)
);
CREATE INDEX idx_recipe_profit_recipe_and_world_id_and_is_hq ON recipe_profit (recipe_id, world_id, is_hq);

CREATE TABLE IF NOT EXISTS price
(
    id                     SERIAL PRIMARY KEY,
    item_id                INTEGER NOT NULL REFERENCES item (id) ON DELETE CASCADE,
    world_id               INTEGER NOT NULL REFERENCES world (id) ON DELETE CASCADE,
    is_hq                  BOOLEAN NOT NULL,
    min_listing_id         INTEGER REFERENCES min_listing (id) ON DELETE CASCADE,
    recent_purchase_id     INTEGER REFERENCES recent_purchase (id) ON DELETE CASCADE,
    average_sale_price_id  INTEGER REFERENCES average_sale_price (id) ON DELETE CASCADE,
    daily_sale_velocity_id INTEGER REFERENCES daily_sale_velocity (id) ON DELETE CASCADE,
    updated                TIMESTAMP WITH TIME ZONE NOT NULL,
    UNIQUE (item_id, world_id, is_hq)
);
CREATE INDEX idx_price_item_id_and_world_id ON price (item_id, world_id);
CREATE INDEX idx_price_item_id_and_world_id_and_is_hq ON price (item_id, world_id, is_hq);