CREATE TABLE IF NOT EXISTS outbox_messages
(
    id uuid PRIMARY KEY,
    type text NOT NULL,
    payload jsonb NOT NULL,
    occurred_at_utc timestamptz NOT NULL,
    created_at_utc  timestamptz NOT NULL DEFAULT now(),
    status smallint NOT NULL DEFAULT 0, -- 0=Pending, 1=InProgress, 2=Published, 3=Failed
    publish_attempts int NOT NULL DEFAULT 0,
    last_attempt_at_utc timestamptz NULL,
    error text NULL
);

CREATE INDEX IF NOT EXISTS ix_outbox_status_created
    ON outbox_messages (status, created_at_utc);