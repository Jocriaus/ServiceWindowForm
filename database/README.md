# Database

Schema for `queueing_system`, used by the queue display (this app) and the teller/admin apps.

## Run order

1. `migrations/001_create_database.sql`
2. `migrations/002_create_user_table.sql`
3. `migrations/003_create_client_table.sql`
4. *(optional, development only)* `seed/dev_seed.sql`: six window users and ten sample clients

Every file can be run again safely: the migrations use `IF NOT EXISTS`, and the seed resets its own rows.
**Never run the seed on the live database.**

## How to run

- **phpMyAdmin:** open the Import tab and import each file in the order above.
- **Command line** (from the repo root):
  ```
  mysql -u root -p -e "source database/migrations/001_create_database.sql"
  ```
  Repeat for each file.

`queries/display_queries.sql` holds the two queries the display runs every second. Paste it into phpMyAdmin to see what the screen will show.
