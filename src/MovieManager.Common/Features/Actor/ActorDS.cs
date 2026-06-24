using MH.Utils.DB;
using MH.Utils.DB.DataSources;
using System;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Actor;

/// <summary>
/// DB fields: Id|Name|Person|Image
/// </summary>
public sealed class ActorDS : CsvRepositoryDataSource<ActorM, ActorR, ActorLinkInfo> {
  private readonly PM.CoreR _pmCoreR;

  public static ActorM Dummy { get; } = new(0, string.Empty);

  public ActorDS(CoreR coreR, PM.CoreR pmCoreR, ActorR repo) : base(coreR.DB, "Actors", 4, repo) {
    _pmCoreR = pmCoreR;
  }

  protected override (ActorM item, ActorLinkInfo linkInfo) _fromCsv(ReadOnlySpan<char> csv) {
    int start = 0;
    int field = 0;

    int id = 0;
    string name = string.Empty;
    int personId = 0;
    int imageId = 0;

    for (int i = 0; i <= csv.Length; i++) {
      if (i == csv.Length || csv[i] == '|') {
        var slice = csv[start..i];

        switch (field) {
          case 0: id = CsvParser.ParseInt(slice); break;
          case 1: name = slice.ToString(); break;
          case 2: personId = CsvParser.ParseInt(slice); break;
          case 3: imageId = CsvParser.ParseInt(slice); break;
        }

        field++;
        start = i + 1;
      }
    }

    _validateFieldsCount(field, csv);

    var item = new ActorM(id, name);
    var linkInfo = new ActorLinkInfo(personId, imageId);

    return new(item, linkInfo);
  }

  protected override string _toCsv(ActorM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.Name,
      item.Person?.GetHashCode().ToString(),
      item.Image?.GetHashCode().ToString());

  public override void LinkReferences() {
    foreach (var (item, li) in _allLinkInfo) {
      item.Person = _pmCoreR.Person.DataSource.GetById(li.PersonId, true);
      item.Image = _pmCoreR.MediaItem.DataSource.GetById(li.ImageId, true);
    }
  }
}

public readonly struct ActorLinkInfo(int personId, int imageId) {
  public int PersonId { get; } = personId;
  public int ImageId { get; } = imageId;
}