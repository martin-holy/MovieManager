using MH.Utils.DB;
using MH.Utils.DB.DataSources;
using System;

namespace MovieManager.Common.Features.Movie;

/// <summary>
/// DB fields: Id|DetailId|DetailName|Movie
/// </summary>
public sealed class MovieDetailIdDS(CoreR coreR, MovieDetailIdR repo)
  : CsvRepositoryDataSource<MovieDetailIdM, MovieDetailIdR, MovieDetailIdLinkInfo>(coreR.DB, "MovieDetailIds", 4, repo) {

  protected override (MovieDetailIdM item, MovieDetailIdLinkInfo linkInfo) _fromCsv(ReadOnlySpan<char> csv) {
    int start = 0;
    int field = 0;

    int id = 0;
    string detailId = string.Empty;
    string detailName = string.Empty;
    int movieId = 0;

    for (int i = 0; i <= csv.Length; i++) {
      if (i == csv.Length || csv[i] == '|') {
        var slice = csv[start..i];

        switch (field) {
          case 0: id = CsvParser.ParseInt(slice); break;
          case 1: detailId = slice.ToString(); break;
          case 2: detailName = slice.ToString(); break;
          case 3: movieId = CsvParser.ParseInt(slice); break;
        }

        field++;
        start = i + 1;
      }
    }

    _validateFieldsCount(field, csv);

    var item = new MovieDetailIdM(id, detailId, detailName, MovieDS.Dummy);
    var linkInfo = new MovieDetailIdLinkInfo(movieId);

    return new(item, linkInfo);
  }

  protected override string _toCsv(MovieDetailIdM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.DetailId,
      item.DetailName,
      item.Movie.GetHashCode().ToString());

  public override void LinkReferences() {
    foreach (var (item, li) in _allLinkInfo) {
      item.Movie = coreR.Movie.DataSource.GetById(li.MovieId)!;
      item.Movie.DetailId = item;
    }
  }
}

public readonly struct MovieDetailIdLinkInfo(int movieId) {
  public int MovieId { get; } = movieId;
}