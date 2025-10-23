using MH.UI.Controls;
using MH.Utils.EventsArgs;
using PictureManager.Common.Features.Segment;
using System;
using System.Collections.Generic;
using PM = PictureManager.Common;

namespace MovieManager.Common.Features.Character;

public class CharacterCollectionView() : CollectionView<CharacterM>(PM.Res.IconPeople, "Characters", [ViewMode.Tiles]) {
  private static readonly IReadOnlyList<SortField<CharacterM>> _sortFields = [
    new SortField<CharacterM>("Name", x => x.Name, StringComparer.CurrentCultureIgnoreCase)
  ];

  public override IEnumerable<GroupByItem<CharacterM>> GetGroupByItems(IEnumerable<CharacterM> source) => [];

  public override int GetItemSize(ViewMode viewMode, CharacterM item, bool getWidth) =>
    getWidth ? 300 : SegmentVM.SegmentUiFullWidth;

  public override IEnumerable<SortField<CharacterM>> GetSortFields() => _sortFields;

  public override int SortCompare(CharacterM itemA, CharacterM itemB) =>
    string.Compare(itemA.Name, itemB.Name, StringComparison.CurrentCultureIgnoreCase);

  protected override void _onItemSelected(SelectionEventArgs<CharacterM> e) =>
    Core.S.Character.Select(e);

  protected override void _onItemOpened(CharacterM item) {
    if (item.Actor.Image != null)
      Core.VM.PMCoreVM.OpenMediaItems(null, item.Actor.Image);
  }

  public override string GetItemTemplateName(ViewMode viewMode) => "MM.Views.CharacterV";
}