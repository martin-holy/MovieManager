using MH.Utils.DB;
using MH.Utils.DB.DataSources;
using MH.Utils.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Movie;

/// <summary>
/// DB fields: Id|Title|Year|YearEnd|Length|Rating|MyRating|Genres|MPAA|Seen|Poster|MediaItems|Keywords|Plot
/// </summary>
public sealed class MovieDS(CoreR coreR, PM.CoreR pmCoreR, MovieR repo)
  : CsvRepositoryDataSource<MovieM, MovieR, MovieLinkInfo>(coreR.DB, "Movies", 14, repo) {
  public static MovieM Dummy { get; } = new(0, string.Empty);

  protected override (MovieM item, MovieLinkInfo linkInfo) _fromCsv(ReadOnlySpan<char> csv) {
    int start = 0;
    int field = 0;

    int id = 0;
    string title = string.Empty;
    int year = 0;
    int? yearEnd = null;
    int length = 0;
    double rating = 0;
    double myRating = 0;
    string genreIds = string.Empty;
    string mpaa = string.Empty;
    ObservableCollection<DateOnly> seen = [];
    int posterId = 0;
    string mediaItemIds = string.Empty;
    string keywordIds = string.Empty;
    string plot = string.Empty;

    for (int i = 0; i <= csv.Length; i++) {
      if (i == csv.Length || csv[i] == '|') {
        var slice = csv[start..i];

        switch (field) {
          case 0: id = CsvParser.ParseInt(slice); break;
          case 1: title = slice.ToString(); break;
          case 2: year = CsvParser.ParseInt(slice); break;
          case 3: yearEnd = slice.IsEmpty ? null : CsvParser.ParseInt(slice); break;
          case 4: length = CsvParser.ParseInt(slice); break;
          case 5: rating = slice.IsEmpty ? 0 : CsvParser.ParseInt(slice) / 10.0; break;
          case 6: myRating = slice.IsEmpty ? 0 : CsvParser.ParseInt(slice) / 10.0; break;
          case 7: genreIds = slice.ToString(); break;
          case 8: mpaa = slice.ToString(); break;
          case 9: seen = slice.IsEmpty
              ? []
              : new ObservableCollection<DateOnly>(
                slice.ToString().Split(',').Select(x => DateOnly.ParseExact(x, "yyyyMMdd", CultureInfo.InvariantCulture)));
            break;
          case 10: posterId = CsvParser.ParseInt(slice); break;
          case 11: mediaItemIds = slice.ToString(); break;
          case 12: keywordIds = slice.ToString(); break;
          case 13: plot = slice.ToString(); break;
        }

        field++;
        start = i + 1;
      }
    }

    _validateFieldsCount(field, csv);

    var item = new MovieM(id, title) {
      Year = year,
      YearEnd = yearEnd,
      Length = length,
      Rating = rating,
      MyRating = myRating,
      MPAA = mpaa,
      Seen = seen,
      Plot = plot
    };

    var linkInfo = new MovieLinkInfo(genreIds, posterId, mediaItemIds, keywordIds);

    return new(item, linkInfo);
  }

  protected override string _toCsv(MovieM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.Title,
      item.Year.ToString(),
      item.YearEnd?.ToString(),
      item.Length.ToString(),
      ((int)(item.Rating * 10)).ToString(),
      ((int)(item.MyRating * 10)).ToString(),
      item.Genres.ToHashCodes().ToCsv(),
      item.MPAA,
      item.Seen.Select(x => x.ToString("yyyyMMdd", CultureInfo.InvariantCulture)).ToCsv(),
      item.Poster?.GetHashCode().ToString(),
      item.MediaItems?.ToHashCodes().ToCsv(),
      item.Keywords?.ToHashCodes().ToCsv(),
      item.Plot);

  public override void LinkReferences() {
    foreach (var (item, li) in _allLinkInfo) {
      item.Genres = coreR.Genre.DataSource.LinkList(li.GenreIds, null, this) ?? [];
      item.Poster = pmCoreR.MediaItem.DataSource.GetById(li.PosterId, true);
      item.MediaItems = pmCoreR.MediaItem.DataSource.Link(li.MediaItemIds);
      item.Keywords = pmCoreR.Keyword.DataSource.Link(li.KeywordIds, this);
    }
  }
}

public readonly struct MovieLinkInfo(string genreIds, int posterId, string mediaItemIds, string keywordIds) {
  public string GenreIds { get; } = genreIds;
  public int PosterId { get; } = posterId;
  public string MediaItemIds { get; } = mediaItemIds;
  public string KeywordIds { get; } = keywordIds;
}