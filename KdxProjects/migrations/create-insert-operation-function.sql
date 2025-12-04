-- Drop existing function if it exists
DROP FUNCTION IF EXISTS insert_operation(
    text, int, int, text, text, text, text,
    text, text, text, text, text, text, text, text, text,
    text, text, text, int, int, text
);

-- Create the insert_operation function matching the actual Operation table schema
CREATE OR REPLACE FUNCTION insert_operation(
    operation_name text,
    cy_id int,
    category_id int,
    go_back text,
    start_val text,
    finish text,
    valve1 text,
    s1 text,
    s2 text,
    s3 text,
    s4 text,
    s5 text,
    ss1 text,
    ss2 text,
    ss3 text,
    ss4 text,
    pil text,
    sc text,
    fc text,
    cycle_id int,
    sort_number int,
    con text
) RETURNS BIGINT AS $$
DECLARE
    new_id BIGINT;
BEGIN
    INSERT INTO "Operation" (
        "OperationName",
        "CYId",
        "CategoryId",
        "GoBack",
        "Start",
        "Finish",
        "Valve1",
        "S1",
        "S2",
        "S3",
        "S4",
        "S5",
        "SS1",
        "SS2",
        "SS3",
        "SS4",
        "PIL",
        "SC",
        "FC",
        "CycleId",
        "SortNumber",
        "Con"
    ) VALUES (
        operation_name,
        cy_id,
        category_id,
        go_back,
        start_val,
        finish,
        valve1,
        s1,
        s2,
        s3,
        s4,
        s5,
        ss1,
        ss2,
        ss3,
        ss4,
        pil,
        sc,
        fc,
        cycle_id,
        sort_number,
        con
    )
    RETURNING "Id" INTO new_id;

    RETURN new_id;
END;
$$ LANGUAGE plpgsql;
