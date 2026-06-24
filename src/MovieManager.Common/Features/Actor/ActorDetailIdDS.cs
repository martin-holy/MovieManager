using MH.Utils.DB;
using MH.Utils.DB.DataSources;
using System;

namespace MovieManager.Common.Features.Actor;

/// <summary>
/// DB fields: Id|DetailId|DetailName|Actor
/// </summary>
public sealed class ActorDetailIdDS(CoreR coreR, ActorDetailIdR repo)
  : CsvRepositoryDataSource<ActorDetailIdM, ActorDetailIdR, ActorDetailIdLinkInfo>(coreR.DB, "ActorDetailIds", 4, repo) {

  protected override (ActorDetailIdM item, ActorDetailIdLinkInfo linkInfo) _fromCsv(ReadOnlySpan<char> csv) {
    int start = 0;
    int field = 0;

    int id = 0;
    string detailId = string.Empty;
    string detailName = string.Empty;
    int actorId = 0;

    for (int i = 0; i <= csv.Length; i++) {
      if (i == csv.Length || csv[i] == '|') {
        var slice = csv[start..i];

        switch (field) {
          case 0: id = CsvParser.ParseInt(slice); break;
          case 1: detailId = slice.ToString(); break;
          case 2: detailName = slice.ToString(); break;
          case 3: actorId = CsvParser.ParseInt(slice); break;
        }

        field++;
        start = i + 1;
      }
    }

    _validateFieldsCount(field, csv);

    var item = new ActorDetailIdM(id, detailId, detailName, ActorDS.Dummy);
    var linkInfo = new ActorDetailIdLinkInfo(actorId);

    return new(item, linkInfo);
  }

  protected override string _toCsv(ActorDetailIdM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.DetailId,
      item.DetailName,
      item.Actor.GetHashCode().ToString());

  public override void LinkReferences() {
    foreach (var (item, li) in _allLinkInfo) {
      item.Actor = coreR.Actor.DataSource.GetById(li.ActorId)!;
      item.Actor.DetailId = item;
    }
  }
}

public readonly struct ActorDetailIdLinkInfo(int actorId) {
  public int ActorId { get; } = actorId;
}