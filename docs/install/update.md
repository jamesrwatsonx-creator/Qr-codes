# Flink Updates

flink is built in a way, that you can safely update from any old version to any newer version (with some exceptions, see below). That's why you can pin to `latest` without headache. Downgrading from a newer version to an older version is not supported, but will work in 90% of the cases (works when no Database migration is involved, fails if the Database Migration is not reversible). So if you'd like to update, make sure to check on a local test flink instance, whether you like the new features and whether you want your productive instance to have those features.


## Breaking Changes

### from 1.x.x to 2.x.x

Flink 1.x.x used `DateTime` for the CreationDate of Shortlinks. `DateTime` is sometimes created by `EntityFramework` as `TEXT`, other times as `BIGINT`, depending on the Database type, and the `EF` version. This is not acceptable to build on top of, because the number of combinations are effectively impossible to factor in when creating new Schema Migrations. To prevent this from happening, Flink Domain models from 2.x.x on only use very basic Datatypes that are supported well across ALL databases. When upgrading to 2.x.x Flink will drop the `CreationDate` column (losing ALL its contents), and create a new CreationDate column of type `BIGINT` instead (with the default value being the point in time when the migration is performed). In case you used the CreationDates, you might want to pull a backup from the Database before updating.