using MH.Utils.DB.Repositories;
using MovieManager.Plugins.Common.DTOs;
using System.Linq;

namespace MovieManager.Common.Features.Actor;

public sealed class ActorDetailIdR : Repository<ActorDetailIdM> {
  public ActorDetailIdDS DataSource { get; }

  public ActorDetailIdR(CoreR coreR) {
    DataSource = new(coreR, this);
  }

  public ActorDetailIdM ItemCreate(DetailId detailId, ActorM actor) =>
    ItemCreate(new(GetNextId(), detailId.Id, detailId.Name, actor));

  public ActorM? GetActor(DetailId detailId) =>
    All.FirstOrDefault(x =>
      x.DetailName.Equals(detailId.Name)
      && x.DetailId.Equals(detailId.Id))?.Actor;
}