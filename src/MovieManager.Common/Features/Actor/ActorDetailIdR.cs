using MH.Utils.BaseClasses;
using MovieManager.Plugins.Common.DTOs;
using System.Linq;

namespace MovieManager.Common.Features.Actor;

/// <summary>
/// DB fields: Id|DetailId|DetailName|Actor
/// </summary>
public sealed class ActorDetailIdR(CoreR coreR) : TableDataAdapter<ActorDetailIdM>(coreR, "ActorDetailIds", 4) {
  protected override ActorDetailIdM _fromCsv(string[] csv) =>
    new(int.Parse(csv[0]), csv[1], csv[2], ActorR.Dummy);

  protected override string _toCsv(ActorDetailIdM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.DetailId,
      item.DetailName,
      item.Actor.GetHashCode().ToString());

  public override void LinkReferences() {
    foreach (var (item, csv) in _allCsv) {
      item.Actor = coreR.Actor.GetById(csv[3])!;
      item.Actor.DetailId = item;
    }
  }

  public ActorDetailIdM ItemCreate(DetailId detailId, ActorM actor) =>
    ItemCreate(new(GetNextId(), detailId.Id, detailId.Name, actor));

  public ActorM? GetActor(DetailId detailId) =>
    All.FirstOrDefault(x =>
      x.DetailName.Equals(detailId.Name)
      && x.DetailId.Equals(detailId.Id))?.Actor;
}