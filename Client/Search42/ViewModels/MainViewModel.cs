using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Azure.Search.Models;
using Search42.Core.Models;
using Search42.Core.Services;
using System.Linq;

namespace Search42.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISearchService searchService;

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => Set(ref searchText, value);
        }

        private DocumentSearchResult<CognitiveSearchResult> searchResult;
        public DocumentSearchResult<CognitiveSearchResult> SearchResult
        {
            get => searchResult;
            set => Set(ref searchResult, value);
        }

        public IEnumerable<CognitiveSearchResult> searchResultItems;
        public IEnumerable<CognitiveSearchResult> SearchResultItems
        {
            get => searchResultItems;
            set => Set(ref searchResultItems, value);
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => Set(ref isBusy, value);
        }

        public RelayCommand SearchCommand { get; set; }

        public MainViewModel(ISearchService searchService)
        {
            this.searchService = searchService;

            SearchCommand = new RelayCommand(async () => await SearchAsync());
        }

        private async Task SearchAsync()
        {
            IsBusy = true;

            SearchResult = await searchService.SearchAsync(searchText, new List<string> { "imageTags" });
            SearchResultItems = searchResult.Results.Select(r => r.Document).ToList();

            IsBusy = false;
        }
    }
}
