## todo

* 컨플루언스 위키 문법 문서 : https://confluence.atlassian.com/doc/confluence-wiki-markup-251003035.html
* 컨플루언스 storage format 문서 : https://confluence.atlassian.com/doc/confluence-storage-format-790796544.html

## backlog

* 이미지 첨부 처리
  * https://confluence.atlassian.com/confkb/using-the-confluence-rest-api-to-upload-an-attachment-to-one-or-more-pages-1014274390.html
  * v2에서는 첨부 동작을 지원하지 않는 듯.
* path 구성 호환을 위해 만들어진 페이지들은 본문에 하위 페이지 목록 추가하기.

## done

### 240413

success:82 failed:24

### 240412

* 링크 처리 : 다른 페이지로 링크는 먼저 간단히 가능해 보인다. WikiJsController 싱글턴으로 변경하고 제목 정보 얻어와서 처리.
* 다른 페이지 링크 연결해주기 - 링크는 path 기반으로 되어있고, 변환할 페이지는 title로 만들어짐.

### 240410

* space 선택하면서 만들어진 페이지 가져올 때 본문 제대로 가져오고 파싱하도록 개선 필요. 
  * 해두지 않으면 convert 다시 돌릴 떄 수정내역이 있는지 확인하는 로직이 제대로 돌지 않는다. 
  * api로 받아온 본문 포맷 변환 통제가 어려워, 별도 버전을 붙이고 버전값을 비교하도록 처리

### 2404xx

* 업데이트 안 되는 페이지 원인 확인 필요.
  * html이 들어있으면 제대로 변환되지 않는다. api가 보안 이슈로 거절한다.
  * https://github.com/baynezy/Html2Markdown?tab=readme-ov-file 이거 이용해서 기본적인 markdown으로 변환하고, 미변환 태그를 몇 개 지워주면 되지 않을까.

### 2403xx

* id 안 붙여서 만든 페이지들 일괄 정리.

### 240325

* 컨플루언스는 타이틀 text가 unique해야 함. 변환하는 페이지에 중복된 이름 있을 때 suffix 붙여야 한다.
* 이미 존재하는 페이지면 본문 비교하고 변경사항 있을 때 update 하도록 처리.