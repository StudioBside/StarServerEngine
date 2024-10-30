## todo

- 다중 선택해서 값 변경하기
- 컷 추가할 때 스크롤 내리기
- 컷템플릿 엑셀 앞에 있던 컬럼... 뭐였더라. 연동하기 (in header)
- color picker 알아보기
- 텍스트에 부분선택해서 색상 쉽게 입히기
```csharp
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string inputText = "색상 적용 전 <color=#FFD34C>[중요 텍스트]</color>색상 적용 후";
            ParseAndApplyColors(inputText);
        }

        private void ParseAndApplyColors(string inputText)
        {
            var richTextBox = new RichTextBox();
            var document = new FlowDocument();
            richTextBox.Document = document;

            string pattern = @"<color=#(?<color>[0-9A-Fa-f]{6})>(?<text>.*?)<\/color>";
            var matches = Regex.Matches(inputText, pattern);

            int lastIndex = 0;
            foreach (Match match in matches)
            {
                // Add text before the match
                if (match.Index > lastIndex)
                {
                    document.Blocks.Add(new Paragraph(new Run(inputText.Substring(lastIndex, match.Index - lastIndex))));
                }

                // Add colored text
                string colorCode = match.Groups["color"].Value;
                string coloredText = match.Groups["text"].Value;
                var run = new Run(coloredText)
                {
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{colorCode}"))
                };
                document.Blocks.Add(new Paragraph(run));

                lastIndex = match.Index + match.Length;
            }

            // Add remaining text after the last match
            if (lastIndex < inputText.Length)
            {
                document.Blocks.Add(new Paragraph(new Run(inputText.Substring(lastIndex))));
            }

            this.Content = richTextBox;
        }
    }
}
```

## error

## done

- 새 컷 추가버튼 분할. 선택지컷 넣는 기능 추가. (maybe split button)
- bgm picker
- fx picker
- voice picker
- listview item 간의 여백 더 좁게 조정
- UnitMotion, TransitionEffect, TransitionControl
- 브랜치컷 표현방식 구상
- jump anchor 처리
- talk append 처리방식 구상
  - 증요도가 높지 않아 디테일 보강은 나중에 하기로.
- 요약 표현 기능 넣고, 일괄 박스 제어 기능 대체
- 드래그 할 때 시인성 높이기
- sound 표시위치에 여백 확보
- unitTemplet 읽기
- unit portrait 로딩 및 출력
- multi select를 부드럽게 다듬어야 한다. 여러 개 선택한 후 재선택할 때 풀리지 않게 하기
- 유닛 포지션 데이터 연동.
