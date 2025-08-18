CREATE TABLE IF NOT EXISTS inbox_consumed
(
    message_id uuid NOT NULL,
    handler text NOT NULL,
    processed_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (message_id, handler)
);