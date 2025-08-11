SELECT * FROM public."Accounts";

CREATE OR REPLACE PROCEDURE public.accrue_interest(account_id UUID)
    LANGUAGE plpgsql
AS $$
DECLARE
    v_balance       numeric(18,2);
    v_interest_rate numeric(5,2);
    v_currency      char(3);
    v_amount        numeric(18,2);
BEGIN
    -- Блокируем строку счета для предотвращения гонок
    SELECT "Balance", "InterestRate", "Currency"
    INTO v_balance, v_interest_rate, v_currency
    FROM public."Accounts"
    WHERE "Id" = account_id
      AND "Type" = 'Deposit'
        FOR UPDATE;

    IF NOT FOUND OR v_interest_rate IS NULL THEN
        RAISE NOTICE 'Account % not found or has no interest rate', account_id;
        RETURN;
    END IF;

    v_amount := ROUND(v_balance * v_interest_rate / 100, 2);

    IF v_amount = 0 THEN
        RAISE NOTICE 'No interest to accrue for account %', account_id;
        RETURN;
    END IF;

    -- Обновляем баланс счета
    UPDATE public."Accounts"
    SET "Balance" = "Balance" + v_amount
    WHERE "Id" = account_id;

    -- Вставляем транзакцию начисления процентов
    INSERT INTO public."Transactions" (
        "Id",
        "AccountId",
        "CounterPartyAccountId",
        "Currency",
        "Amount",
        "Type",
        "Description",
        "CreatedAt"
    )
    VALUES (
               gen_random_uuid(),
               account_id,
               NULL,
               v_currency,
               v_amount,
               'Credit',
               'Начисление процентов',
               CURRENT_TIMESTAMP
           );

    RAISE NOTICE 'Accrued % % to account %', v_amount, v_currency, account_id;
END;
$$;

Drop Procedure accrue_interest(uuid);

CALL public.accrue_interest('55555555-5555-5555-5555-555555555555');


SELECT proname, prosrc, nspname
FROM pg_proc
         JOIN pg_namespace ON pg_proc.pronamespace = pg_namespace.oid
WHERE proname = 'accrue_interest';