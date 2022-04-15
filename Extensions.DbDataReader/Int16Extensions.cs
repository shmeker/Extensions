using System;

namespace Extensions.DbDataReader
{
	/// <summary>
	/// Static class with methods to read data as short.
	/// </summary>
	public static class Int16Extensions
	{
		/// <summary>
		/// Extension method to get short value from reader.
		/// </summary>
		/// <param name="dbDataReader"><see cref="System.Data.Common.DbDataReader">DbDataReader</see> object to read data from.</param>
		/// <param name="columnOrdinal">Ordinal number of the column.</param>
		/// <returns>Value converted as short if exists, null otherwise.</returns>
		/// <exception cref="ArgumentNullException">Throws if <see cref="System.Data.Common.DbDataReader">DbDataReader</see> object is null.</exception>
		public static short? GetInt16Nullable(this System.Data.Common.DbDataReader dbDataReader, int columnOrdinal)
		{
			if (dbDataReader == null)
			{
				throw new ArgumentNullException(nameof(dbDataReader));
			}

			if (dbDataReader.IsDBNull(columnOrdinal))
			{
				return null;
			}

			return dbDataReader.GetInt16(columnOrdinal);
		}

		/// <summary>
		/// Extension method to get short value from reader.
		/// </summary>
		/// <param name="dbDataReader"><see cref="System.Data.Common.DbDataReader">DbDataReader</see> object to read data from.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>Value converted as short if exists, null otherwise.</returns>
		/// <exception cref="ArgumentNullException">Throws if parameter columnName is null or white space.</exception>
		public static short? GetInt16Nullable(this System.Data.Common.DbDataReader dbDataReader, string columnName)
		{
			if (string.IsNullOrWhiteSpace(columnName))
			{
				throw new ArgumentNullException(nameof(columnName));
			}

			int columnOrdinal = dbDataReader.GetOrdinal(columnName);

			return dbDataReader.GetInt16Nullable(columnOrdinal);
		}
	}
}
