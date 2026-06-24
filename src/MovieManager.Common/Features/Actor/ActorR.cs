using MH.Utils.DB.Repositories;
using PictureManager.Common.Features.MediaItem;
using PictureManager.Common.Features.Person;
using System;
using System.Linq;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Actor;

public sealed class ActorR : Repository<ActorM> {
  public ActorDS DataSource { get; }

  public ActorR(CoreR coreR, PM.CoreR pmCoreR) {
    DataSource = new(coreR, pmCoreR, this);
  }

  public event EventHandler<ActorM> ActorPersonChangedEvent = delegate { };

  public ActorM ItemCreate(string name) =>
    ItemCreate(new ActorM(GetNextId(), name));

  public void SetPerson(ActorM actor, PersonM? person) {
    actor.Person = person;
    IsModified = true;
    ActorPersonChangedEvent(this, actor);
  }

  public void OnMediaItemDeleted(MediaItemM mi) {
    foreach (var actor in All.Where(x => ReferenceEquals(x.Image, mi))) {
      actor.Image = null;
      IsModified = true;
    }
  }

  public void OnPersonDeleted(PersonM person) {
    foreach (var actor in All.Where(x => ReferenceEquals(x.Person, person)))
      SetPerson(actor, null);
  }
}