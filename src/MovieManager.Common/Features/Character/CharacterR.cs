using MH.Utils.DB.Repositories;
using MovieManager.Common.Features.Actor;
using MovieManager.Common.Features.Movie;
using PictureManager.Common.Features.Segment;
using System.Linq;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Character;

public sealed class CharacterR : Repository<CharacterM> {
  public CharacterDS DataSource { get; }

  public CharacterR(CoreR coreR, PM.CoreR pmCoreR) {
    DataSource = new(coreR, pmCoreR, this);
  }

  public CharacterM ItemCreate(string name, ActorM actor, MovieM movie) =>
    ItemCreate(new(GetNextId(), name, actor, movie));

  public void SetSegment(CharacterM character, SegmentM? segment) {
    if (segment == null) return;
    character.Segment = segment;
    IsModified = true;
  }

  public void OnSegmentDeleted(SegmentM segment) {
    foreach (var character in All.Where(x => ReferenceEquals(x.Segment, segment))) {
      character.Segment = null;
      IsModified = true;
    }
  }
}