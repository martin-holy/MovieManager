using MH.Utils.BaseClasses;
using MovieManager.Plugins.Common.DTOs;
using System.Linq;

namespace MovieManager.Common.Features.Movie;

/// <summary>
/// DB fields: Id|DetailId|DetailName|Movie
/// </summary>
public sealed class MovieDetailIdR(CoreR coreR) : TableDataAdapter<MovieDetailIdM>(coreR, "MovieDetailIds", 4) {
  protected override MovieDetailIdM _fromCsv(string[] csv) =>
    new(int.Parse(csv[0]), csv[1], csv[2], MovieR.Dummy);

  protected override string _toCsv(MovieDetailIdM item) =>
    string.Join("|",
      item.GetHashCode().ToString(),
      item.DetailId,
      item.DetailName,
      item.Movie.GetHashCode().ToString());

  public override void LinkReferences() {
    foreach (var (item, csv) in AllCsv) {
      item.Movie = coreR.Movie.GetById(csv[3])!;
      item.Movie.DetailId = item;
    }
  }

  public MovieDetailIdM ItemCreate(DetailId detailId, MovieM movie) =>
    ItemCreate(new(GetNextId(), detailId.Id, detailId.Name, movie));

  public MovieM? GetMovie(DetailId detailId) =>
    All.FirstOrDefault(x =>
      x.DetailName.Equals(detailId.Name)
      && x.DetailId.Equals(detailId.Id))?.Movie;
}