using System;

namespace Extensions.DbDataReader
{
	/// <summary>
	/// Static class with methods to read data as double.
	/// </summary>
	public static class DoubleExtensions
	{
		/// <summary>
		/// Extension method to get double value from reader.
		/// </summary>
		/// <param name="dbDataReader"><see cref="System.Data.Common.DbDataReader">DbDataReader</see> object to read data from.</param>
		/// <param name="columnOrdinal">Ordinal number of the column.</param>
		/// <returns>Value converted as double if exists, null otherwise.</returns>
		/// <exception cref="ArgumentNullException">Throws if <see cref="System.Data.Common.DbDataReader">DbDataReader</see> object is null.</exception>
		public static double? GetDoubleNullable(this System.Data.Common.DbDataReader dbDataReader, int columnOrdinal)
		{
			if (dbDataReader == null)
			{
				throw new ArgumentNullException(nameof(dbDataReader));
			}

			if (dbDataReader.IsDBNull(columnOrdinal))
			{
				return null;
			}

			return dbDataReader.GetDouble(columnOrdinal);
		}

		/// <summary>
		/// Extension method to get double value from reader.
		/// </summary>
		/// <param name="dbDataReader"><see cref="System.Data.Common.DbDataReader">DbDataReader</see> object to read data from.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>Value converted as double if exists, null otherwise.</returns>
		/// <exception cref="ArgumentNullException">Throws if parameter columnName is null or white space.</exception>
		public static double? GetDoubleNullable(this System.Data.Common.DbDataReader dbDataReader, string columnName)
		{
			if (string.IsNullOrWhiteSpace(columnName))
			{
				throw new ArgumentNullException(nameof(columnName));
			}

			int columnOrdinal = dbDataReader.GetOrdinal(columnName);

			return dbDataReader.GetDoubleNullable(columnOrdinal);
		}
	}
}
