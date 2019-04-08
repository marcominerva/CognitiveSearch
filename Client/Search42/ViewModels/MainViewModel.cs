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

        private IEnumerable<Facet> facets;
        public IEnumerable<Facet> Facets
        {
            get => facets;
            set => Set(ref facets, value);
        }

        private Facet selectedFacet;
        public Facet SelectedFacet
        {
            get => selectedFacet;
            set
            {
                if (value != null && Set(ref selectedFacet, value))
                {
                    var task = SearchAsync();
                }
            }
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

            SearchResult = await searchService.SearchAsync(searchText,
                filters: selectedFacet != null ? $"imageTags/any(t: t eq '{selectedFacet.Key}')" : null,
                facets: new List<string> { "imageTags" });

            if (selectedFacet == null)
            {
                Facets = searchResult.Facets.FirstOrDefault().Value.Select(f => new Facet { Key = f.Value.ToString(), Count = f.Count.GetValueOrDefault() });
            }

            SearchResultItems = searchResult.Results.Select(r => r.Document).ToList();

            selectedFacet = null;
            IsBusy = false;
        }
    }
}
