using GalaSoft.MvvmLight;
using Microsoft.Azure.Search.Models;
using Search42.Common;
using Search42.Core.Models;
using Search42.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Search42.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISearchService searchService;

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => Set(ref searchText, value, broadcast: true);
        }

        private DocumentSearchResult<CognitiveSearchResult> searchResult;
        public DocumentSearchResult<CognitiveSearchResult> SearchResult
        {
            get => searchResult;
            set
            {
                if (Set(ref searchResult, value))
                {
                    RaisePropertyChanged(nameof(SearchResultItems));
                }
            }
        }

        public IEnumerable<CognitiveSearchResult> SearchResultItems => searchResult?.Results.Select(r => r.Document);

        private IEnumerable<Facet> facets;
        public IEnumerable<Facet> Facets
        {
            get => facets;
            set => Set(ref facets, value);
        }

        private IEnumerable<string> suggestions;
        public IEnumerable<string> Suggestions
        {
            get => suggestions;
            set => Set(ref suggestions, value);
        }

        private Facet selectedFacet;
        public Facet SelectedFacet
        {
            get => selectedFacet;
            set
            {
                if (value != null && Set(ref selectedFacet, value))
                {
                    SearchCommand.Execute(searchText);
                }
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => Set(ref isBusy, value, broadcast: true);
        }

        public AutoRelayCommand<string> SearchCommand { get; }

        public AutoRelayCommand<bool> TextChangedCommand { get; }

        public MainViewModel(ISearchService searchService)
        {
            this.searchService = searchService;

            SearchCommand = new AutoRelayCommand<string>(async (queryText) => await SearchAsync(queryText),
                (queryText) => !IsBusy)
                .DependsOn(nameof(IsBusy));

            TextChangedCommand = new AutoRelayCommand<bool>(async (isUserInput) => await SuggestAsync(),
                (isUserInput) => isUserInput && searchText?.Length >= 2)
                .DependsOn(nameof(SearchText));
        }

        private async Task SearchAsync(string queryText)
        {
            IsBusy = true;
            SearchText = queryText;

            SearchResult = await searchService.SearchAsync(searchText,
                filters: selectedFacet != null ? $"imageTags/any(t: t eq '{selectedFacet.Key}')" : null,
                facets: new List<string> { "imageTags" });

            if (selectedFacet == null)
            {
                Facets = searchResult.Facets.FirstOrDefault().Value.Select(f => new Facet
                {
                    Key = f.Value.ToString(),
                    Count = f.Count.GetValueOrDefault()
                });
            }

            selectedFacet = null;
            IsBusy = false;
        }

        private async Task SuggestAsync()
        {
            var suggestionReults = await searchService.GetSuggestionsAsync(searchText, "entities");
            Suggestions = suggestionReults?.SelectMany(s => s.Tags)
                .Where(s => s.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase))
                .Distinct().OrderBy(s => s);
        }
    }
}
