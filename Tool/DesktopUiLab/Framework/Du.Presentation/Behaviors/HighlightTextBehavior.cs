namespace Du.Presentation.Behaviors;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

public sealed class HighlightTextBehavior : Behavior<TextBlock>
{
    // Text DependencyProperty
    private static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(HighlightTextBehavior),
            new PropertyMetadata(string.Empty, OnTextOrKeywordChanged));

    // Keyword DependencyProperty
    private static readonly DependencyProperty KeywordListProperty =
        DependencyProperty.Register(
            nameof(KeywordList),
            typeof(IEnumerable<string>),
            typeof(HighlightTextBehavior),
            new PropertyMetadata(Enumerable.Empty<string>(), OnTextOrKeywordChanged));

    public string Text
    {
        get => (string)this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    public IEnumerable<string> KeywordList
    {
        get => (IEnumerable<string>)this.GetValue(KeywordListProperty);
        set => this.SetValue(KeywordListProperty, value);
    }

    //// --------------------------------------------------------------------------------------------
    protected override void OnAttached()
    {
        base.OnAttached();
        this.UpdateTextBlock();
    }

    // TextBlock 갱신 처리
    private static void OnTextOrKeywordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HighlightTextBehavior behavior)
        {
            behavior.UpdateTextBlock();
        }
    }

    // TextBlock 업데이트 로직
    private void UpdateTextBlock()
    {
        if (this.AssociatedObject == null || string.IsNullOrEmpty(this.Text))
        {
            return;
        }

        this.AssociatedObject.Inlines.Clear();

        string fullText = this.Text;
        if (this.KeywordList.Any() == false)
        {
            // 키워드가 없으면 전체 텍스트만 추가
            this.AssociatedObject.Inlines.Add(new Run(fullText));
            return;
        }

        int currentIndex = 0;
        var keywordList = this.KeywordList.OrderByDescending(k => k.Length).ToList(); // 긴 키워드부터 처리

        while (currentIndex < fullText.Length)
        {
            int nextKeywordIndex = -1;
            string nextKeyword = string.Empty;

            // 다음 키워드 찾기
            foreach (var keyword in keywordList)
            {
                int keywordIndex = fullText.IndexOf(keyword, currentIndex, StringComparison.OrdinalIgnoreCase);
                if (keywordIndex >= 0 && (nextKeywordIndex == -1 || keywordIndex < nextKeywordIndex))
                {
                    nextKeywordIndex = keywordIndex;
                    nextKeyword = keyword;
                }
            }

            if (nextKeywordIndex < 0)
            {
                // 키워드가 더 이상 없으면 나머지 텍스트 추가
                this.AssociatedObject.Inlines.Add(new Run(fullText.Substring(currentIndex)));
                break;
            }

            // 키워드 이전 텍스트 추가
            if (nextKeywordIndex > currentIndex)
            {
                this.AssociatedObject.Inlines.Add(new Run(fullText.Substring(currentIndex, nextKeywordIndex - currentIndex)));
            }

            // 키워드 강조 처리
            var highlightRun = new Run(fullText.Substring(nextKeywordIndex, nextKeyword.Length))
            {
                Background = Brushes.Yellow,
                Foreground = Brushes.Black,
            };
            this.AssociatedObject.Inlines.Add(highlightRun);

            currentIndex = nextKeywordIndex + nextKeyword.Length;
        }
    }
}
