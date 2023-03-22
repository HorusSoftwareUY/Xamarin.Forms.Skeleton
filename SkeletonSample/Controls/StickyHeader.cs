using System;
using System.Runtime.CompilerServices;

namespace SkeletonSample.Controls
{
    public enum StickyHeaderAnimations
    {
        Translation, Resize
    }

    public class StickyHeader : ContentView
    {
        #region Attributes

        private RowDefinition rowDefinition;

        private Grid externalGrid;

        private Grid internalGrid;

        private ScrollView scrollView;

        private View currentHeader;

        private View currentContent;

        #endregion Attributes

        #region Properties

        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(View), typeof(StickyHeader), defaultValue: null);

        public View Header
        {
            get => (View)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static new readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(StickyHeader), defaultValue: null);

        public new View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(nameof(HeaderHeight), typeof(double), typeof(StickyHeader), defaultValue: 100.0);

        public double HeaderHeight
        {
            get => (double)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        public static readonly BindableProperty MinimumHeaderHeightProperty = BindableProperty.Create(nameof(MinimumHeaderHeight), typeof(double), typeof(StickyHeader), defaultValue: 50.0);

        public double MinimumHeaderHeight
        {
            get => (double)GetValue(MinimumHeaderHeightProperty);
            set => SetValue(MinimumHeaderHeightProperty, value);
        }

        public static readonly BindableProperty AnimationProperty = BindableProperty.Create(nameof(Animation), typeof(StickyHeaderAnimations), typeof(StickyHeader), defaultValue: StickyHeaderAnimations.Translation);

        public StickyHeaderAnimations Animation
        {
            get => (StickyHeaderAnimations)GetValue(AnimationProperty);
            set => SetValue(AnimationProperty, value);
        }

        #endregion Properties

        #region Constructors

        public StickyHeader()
        {
            this.externalGrid = new Grid { ColumnSpacing = 0, RowSpacing = 0 };

            this.scrollView = new ScrollView();
            this.scrollView.Scrolled += this.ScrollViewScrolled;
            this.externalGrid.Add(this.scrollView, 0, 0);

            this.internalGrid = new Grid { ColumnSpacing = 0, RowSpacing = 0 };
            this.scrollView.Content = this.internalGrid;

            this.rowDefinition = new RowDefinition { Height = new GridLength(this.HeaderHeight, GridUnitType.Absolute) };
            this.internalGrid.RowDefinitions.Add(this.rowDefinition);

            this.internalGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            base.Content = this.externalGrid;
        }

        #endregion Constructors

        #region Operations

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(this.Header):
                    if (this.currentHeader != null)
                    {
                        this.externalGrid.Children.Remove(this.currentHeader);
                    }

                    if (this.Header != null)
                    {
                        this.currentHeader = this.Header;
                        this.currentHeader.HeightRequest = this.HeaderHeight;
                        this.currentHeader.VerticalOptions = LayoutOptions.Start;
                        this.externalGrid.Add(this.currentHeader, 0, 0);
                    }
                    break;
                case nameof(this.Content):
                    if (this.currentContent != null)
                    {
                        this.internalGrid.Children.Remove(this.currentContent);
                    }

                    if (this.Content != null)
                    {
                        this.currentContent = this.Content;
                        this.internalGrid.Add(this.currentContent, 0, 1);
                    }
                    break;
                case nameof(this.HeaderHeight):
                    if (this.currentHeader != null)
                    {
                        this.currentHeader.HeightRequest = this.HeaderHeight;
                    }
                    this.rowDefinition.Height = new GridLength(this.HeaderHeight, GridUnitType.Absolute);
                    break;
            }
        }

        private void ScrollViewScrolled(object sender, ScrolledEventArgs e)
        {
            var scrollY = ((ScrollView)sender).ScrollY;
            if (this.Animation == StickyHeaderAnimations.Translation)
            {
                if (scrollY < this.MinimumHeaderHeight)
                {
                    this.currentHeader.TranslationY = 0 - scrollY;
                }
            }
            else
            {
                var height = this.HeaderHeight - scrollY;
                if (height > this.MinimumHeaderHeight)
                {
                    this.currentHeader.HeightRequest = height;
                }
            }
        }

        #endregion Operations
    }
}