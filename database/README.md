# Database

Schema for `queueing_system`, used by the queue display (this app) and the teller/admin apps.

## Run order

1. `migrations/001_create_database.sql`
2. `migrations/002_create_user_table.sql`
3. `migrations/003_create_client_table.sql`
4. *(optional, development only)* `seed/dev_seed.sql`: six window users and ten sample clients

Every file can be run again safely: the migrations use `IF NOT EXISTS`, and the seed resets its own rows.
**Never run the seed on the live database.**

### Existing databases

If `user` and `client` already exist, migrations 002 and 003 **do nothing**. `IF NOT EXISTS` skips them, so an older table keeps its columns, types and indexes. Before relying on the display's ordering, check the live table:

```
SHOW CREATE TABLE client;            -- TicketNo should be INT (a text type sorts "10" before "9")
SELECT DISTINCT Windows FROM `user`; -- should be: Window 1, Window 2, Cashier, Admission, DSA Office, Bookshop
```

A window name outside that list (for example "Window 3") has no label on the display.

## How to run

- **phpMyAdmin:** open the Import tab and import each file in the order above.
- **Command line** (from the repo root):
  ```
  mysql -u root -p -e "source database/migrations/001_create_database.sql"
  ```
  Repeat for each file.

`queries/display_queries.sql` holds the two queries the display runs every second. Paste it into phpMyAdmin to see what the screen will show.
