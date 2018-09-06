using Ooui;
using System;
using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	internal class Pagination : StylableElement
	{
		/*
<nav aria-label="Page navigation example">
  <ul class="pagination justify-content-end">
    <li class="page-item disabled">
      <a class="page-link" href="#" tabindex="-1">Previous</a>
    </li>
    <li class="page-item"><a class="page-link" href="#">1</a></li>
    <li class="page-item"><a class="page-link" href="#">2</a></li>
    <li class="page-item"><a class="page-link" href="#">3</a></li>
    <li class="page-item">
      <a class="page-link" href="#">Next</a>
    </li>
  </ul>
</nav>
		 */
		private readonly List _list;
		private readonly ListItem _nextListItem;
		private readonly int _pageAmount;
		private readonly List<(int, ListItem)> _pageListItems = new List<(int, ListItem)>();
		private readonly ListItem _prevListItem;
		private readonly Action<int> _switchPageCallback;
		private int _currentPageNumber;
		private int _drawnPageAmount;
		private int _maxPages;
		private int _startAtPage;

		public Pagination(Action<int> switchPageCallback, int maxPages, int pageAmount = 5) : base("nav")
		{
			_switchPageCallback = switchPageCallback;
			_maxPages = maxPages;
			_pageAmount = pageAmount;
			_list = new List(false) { ClassName = "pagination justify-content-end" };

			#region PreviousButton

			_prevListItem = new ListItem() { ClassName = "page-item disabled" };
			Anchor prevAnchor = new Anchor("#", "Zurück") { ClassName = "page-link" };
			_prevListItem.AppendChild(prevAnchor);
			_list.AppendChild(_prevListItem);
			prevAnchor.Click += (sender, args) => SwitchPage(_currentPageNumber - 1);

			#endregion PreviousButton

			#region NextButton

			_nextListItem = new ListItem() { ClassName = "page-item" };
			Anchor nextAnchor = new Anchor("#", "Vor") { ClassName = "page-link" };
			_nextListItem.AppendChild(nextAnchor);
			_list.AppendChild(_nextListItem);
			nextAnchor.Click += (sender, args) => SwitchPage(_currentPageNumber + 1);

			#endregion NextButton

			Redraw(1, pageAmount, 1);
			AppendChild(_list);
		}

		public void ChangeMaxPages(int maxPages)
		{
			_maxPages = maxPages;
			Redraw(_currentPageNumber, _pageAmount, _startAtPage);
			if (_currentPageNumber > _maxPages)
			{
				SwitchPage(_maxPages);
			}
			if (_currentPageNumber <= 0 && _maxPages > 0)
			{
				SwitchPage(1);
			}
		}

		public void Redraw(int currentPageNumber, int pageAmount, int startAtPage)
		{
			pageAmount = Math.Min(_maxPages, pageAmount);
			_currentPageNumber = currentPageNumber;
			_drawnPageAmount = pageAmount;
			_startAtPage = startAtPage;

			while (_drawnPageAmount - (_currentPageNumber - _startAtPage + 1) > (_drawnPageAmount / 2) && _startAtPage > 1)
			{
				_startAtPage--;
			}
			while (_drawnPageAmount - (_currentPageNumber - _startAtPage + 1) < (_drawnPageAmount / 2) && _startAtPage + _drawnPageAmount <= _maxPages)
			{
				_startAtPage++;
			}

			foreach ((int, ListItem) tuple in _pageListItems)
			{
				_list.RemoveChild(tuple.Item2);
			}
			_pageListItems.Clear();

			for (int i = _startAtPage; i < _drawnPageAmount + _startAtPage; i++)
			{
				int localPageNumber = i;
				ListItem listItem = new ListItem
				{
					ClassName = "page-item" + (localPageNumber == _currentPageNumber ? " active" : "")
				};

				Anchor anchor = new Anchor("#", localPageNumber.ToString())
				{
					ClassName = "page-link"
				};
				anchor.Click += (sender, args) => SwitchPage(localPageNumber);
				listItem.AppendChild(anchor);
				_list.InsertBefore(listItem, _nextListItem);
				_pageListItems.Add((localPageNumber, listItem));
			}
			SetActivationNextButton(_currentPageNumber < (_maxPages));
			SetActivationPrevButton(_currentPageNumber > 1);
		}

		public void SetActivationNextButton(bool active)
		{
			if (active)
			{
				_nextListItem.ClassName = _nextListItem.ClassName.Replace(" disabled", "");
			}
			else
			{
				_nextListItem.ClassName += " disabled";
			}
		}

		public void SetActivationPrevButton(bool active)
		{
			if (active)
			{
				_prevListItem.ClassName = _prevListItem.ClassName.Replace(" disabled", "");
			}
			else
			{
				_prevListItem.ClassName += " disabled";
			}
		}

		private void SwitchPage(int pageNumber)
		{
			Redraw(pageNumber, _pageAmount, _startAtPage);
			_switchPageCallback(pageNumber);
		}
	}
}
