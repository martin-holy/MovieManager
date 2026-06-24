using MH.Utils.DB;
using MH.Utils.DB.DataSources;
using MovieManager.Common.Features.Actor;
using MovieManager.Common.Features.Movie;
using System;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Character;

/// <summary>
/// DB fields: Id|Name|Actor|Movie|Segment
/// </summary>
public sealed class CharacterDS(CoreR coreR, PM.CoreR pmCoreR, CharacterR repo)
  : CsvRepositoryDataSource<CharacterM, CharacterR, CharacterLinkInfo>(coreR.DB, "Characters", 5, repo) {
  
  protected override (CharacterM item, CharacterLinkInfo linkInfo) _fromCsv(ReadOnlySpan<char> csv) {
    int start = 0;
    int field = 0;

    int id = 0;
    string name = string.Empty;
    int actorId = 0;
    int movieId = 0;
    int segmentId = 0;

    for (int i = 0; i <= csv.Length; i++) {
      if (i == csv.Length || csv[i] == '|') {
        var slice = csv[start..i];

        switch (field) {
          case 0: id = CsvParser.ParseInt(slice); break;
          case 1: name = slice.ToString(); break;
          case 2: actorId = CsvParser.ParseInt(slice); break;
          case 3: movieId = CsvParser.ParseInt(slice); break;
          case 4: segmentId = CsvParser.ParseInt(slice); break;
        }

        field++;
        start = i + 1;
      }
    }

    _validateFieldsCount(field, csv);

    var item = new CharacterM(id, name, ActorDS.Dummy, MovieDS.Dummy);
    var linkInfo = new CharacterLinkInfo(actorId, movieId, segmentId);

    return new(item, linkInfo);
  }

  protected override string _toCsv(CharacterM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.Name,
      item.Actor.GetHashCode().ToString(),
      item.Movie.GetHashCode().ToString(),
      item.Segment?.GetHashCode().ToString());

  public override void LinkReferences() {
    foreach (var (item, li) in _allLinkInfo) {
      item.Actor = coreR.Actor.DataSource.GetById(li.ActorId)!;
      item.Movie = coreR.Movie.DataSource.GetById(li.MovieId)!;
      item.Segment = pmCoreR.Segment.DataSource.GetById(li.SegmentId, true);
    }
  }
}

public readonly struct CharacterLinkInfo(int actorId, int movieId, int segmentId) {
  public int ActorId { get; } = actorId;
  public int MovieId { get; } = movieId;
  public int SegmentId { get; } = segmentId;
}