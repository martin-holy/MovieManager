using MH.Utils.DB;
using MH.Utils.DB.DataSources;
using System;

namespace MovieManager.Common.Features.Genre;

/// <summary>
/// DB fields: Id|Name
/// </summary>
public sealed class GenreDS(CoreR coreR, GenreR repo)
  : CsvRepositoryDataSource<GenreM, GenreR, NoLinkInfo>(coreR.DB, "Genres", 2, repo) {

  protected override (GenreM item, NoLinkInfo linkInfo) _fromCsv(ReadOnlySpan<char> csv) {
    int start = 0;
    int field = 0;

    int id = 0;
    string name = string.Empty;

    for (int i = 0; i <= csv.Length; i++) {
      if (i == csv.Length || csv[i] == '|') {
        var slice = csv[start..i];

        switch (field) {
          case 0: id = CsvParser.ParseInt(slice); break;
          case 1: name = slice.ToString(); break;
        }

        field++;
        start = i + 1;
      }
    }

    _validateFieldsCount(field, csv);

    var item = new GenreM(id, name);

    return new(item, default);
  }

  protected override string _toCsv(GenreM item) =>
    string.Join("|", item.GetHashCode().ToString(), item.Name);
}