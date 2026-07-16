-- ============================================================
-- Touch feed: ALL offline touches, deduped on natural key : {source, contact_number_clean, touch_utc}
-- MySQL owns attribution/windowing — do NOT pre-collapse per phone.
-- touch_utc derived from UnixDate (ms) — do not pass Date text through
-- (EF Core writes subsecond tails MySQL DATETIME would truncate).
-- ============================================================
SELECT DISTINCT source, contact_number_clean, touch_utc
FROM (
    SELECT
        Source                                                    AS source,
        printf('%010d', PhoneNumber)                              AS contact_number_clean,
        strftime('%Y-%m-%d %H:%M:%S', UnixDate/1000, 'unixepoch') AS touch_utc
    FROM PlumbingEntities
    WHERE PhoneNumber > 0 
        AND UnixDate >  unixepoch('2000-01-01 00:00:00', 'subsec') * 1000 -- Date is after 2000-01-01, which was before any source existed
        AND UnixDate <= unixepoch('now',                 'subsec') * 1000 -- Date is before right now

    UNION ALL

    SELECT
        UtmSource                                                 AS source,
        printf('%010d', PhoneNumber)                              AS contact_number_clean,
        strftime('%Y-%m-%d %H:%M:%S', UnixDate/1000, 'unixepoch') AS touch_utc
    FROM CornEntities
    WHERE PhoneNumber > 0
    -- This is here to remove these until we completely nail down the cornentities side of this query
    -- This column has a NOT NULL constraint as of 2026-07-09
      AND Source is NULL
)
ORDER BY touch_utc, contact_number_clean;
