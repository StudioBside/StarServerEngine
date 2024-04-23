## todo

* 컨플루언스 위키 문법 문서 : https://confluence.atlassian.com/doc/confluence-wiki-markup-251003035.html
* 컨플루언스 storage format 문서 : https://confluence.atlassian.com/doc/confluence-storage-format-790796544.html

## backlog

* 이미지 첨부 처리
  * https://confluence.atlassian.com/confkb/using-the-confluence-rest-api-to-upload-an-attachment-to-one-or-more-pages-1014274390.html
  * v2에서는 첨부 동작을 지원하지 않는 듯.
* path 구성 호환을 위해 만들어진 페이지들은 본문에 하위 페이지 목록 추가하기.

## done

### 240423

success:190 failed:6
failed page summary
32 : 클라이언트 빌드 이슈 기록. 본문이 너무 긴 듯.
41 : video 태그 쓰였음
42 : video 태그 쓰였음
65 : video 태그 쓰였음
93 : video 태그 쓰였음
206 : todo 형식 list 양식이 쓰임.

Failed to create page: 클라이언트 빌드 이슈 기록 (32) statusCode:BadRequest (CfPage.cs:57)
Failed to create page: Tech/ClientBuildHistory conentType:html (CfSpace.cs:111)
Page not found: /Project/MeetingLog/202206/20220610_%EB%8B%A8%EA%B8%B0%EC%9D%BC%EC%A0%95 (PageLinkReplacer.cs:45)
Failed to create page: 2022. 06. 작업 기록 (41) statusCode:BadRequest (CfPage.cs:57)
Failed to create page: Tech/Milestone/202206_record conentType:markdown (CfSpace.cs:111)
Failed to create page: html 편집기 테스트 (42) statusCode:BadRequest (CfPage.cs:57)
Failed to create page: WikiJs/htmlTest conentType:html (CfSpace.cs:111)
Page not found:  (PageLinkReplacer.cs:45)
Failed to create page: 2022. 08. 작업 기록 (65) statusCode:BadRequest (CfPage.cs:57)
Failed to create page: Tech/Milestone/202208_record conentType:markdown (CfSpace.cs:111)
Page not found: /ko/%EC%95%84%ED%8A%B8/%EC%95%A0%EB%8B%88%EB%A9%94%EC%9D%B4%EC%85%98/ani_work_flow (PageLinkReplacer.cs:45)
Page not found: /e/ko/%EC%95%84%ED%8A%B8/%EC%95%A0%EB%8B%88%EB%A9%94%EC%9D%B4%EC%85%98/ani_max_work_rule (PageLinkReplacer.cs:45)
Page not found: /ko/%EC%95%84%ED%8A%B8/%EC%95%A0%EB%8B%88%EB%A9%94%EC%9D%B4%EC%85%98/ani_unity_work_flow (PageLinkReplacer.cs:45)
Page not found: /ko/%EC%95%84%ED%8A%B8/%EC%9D%B4%ED%8E%99%ED%8A%B8/%ED%85%8D%EC%8A%A4%EC%B3%90/%EC%9E%84%ED%8F%AC%ED%8A%B8 (PageLinkReplacer.cs:45)
Failed to create page: 2022. 09. 작업 기록 (93) statusCode:BadRequest (CfPage.cs:57)
Failed to create page: Tech/Milestone/202209_record conentType:markdown (CfSpace.cs:111)
Page not found: mailto:studiobside01+11@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+6@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+66@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+16@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+17@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+36@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+28@gmail.com (PageLinkReplacer.cs:45)
Page not found: mailto:studiobside01+4@gmail.com (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/Unity (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/P4 (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/Unity (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/P4 (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/VSCODE (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/p4error (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/Unity (PageLinkReplacer.cs:45)
Page not found: /ko/Newbie/Manual/P4 (PageLinkReplacer.cs:45)
Page not found: /Tech/ProcessTurnCodeReview (PageLinkReplacer.cs:45)
Page not found: www.google.com (PageLinkReplacer.cs:45)
Page not found: /ko/%EC%95%84%ED%8A%B8/UI/%EC%BB%A8%ED%85%90%EC%B8%A0/%EC%9C%A0%EB%8B%9B%ED%8F%AC%ED%8A%B8%EB%A0%88%EC%9D%B4%ED%8A%B8 (PageLinkReplacer.cs:45)
Page not found: /ko/%EC%95%84%ED%8A%B8/UI/%EC%BB%A8%ED%85%90%EC%B8%A0/%EC%8A%A4%ED%82%AC%EC%95%84%EC%9D%B4%EC%BD%98 (PageLinkReplacer.cs:45)
Page not found: /%EC%95%84%ED%8A%B8/UI/%EC%BB%A8%ED%85%90%EC%B8%A0/%EC%95%84%EC%9D%B4%ED%85%9C%EC%95%84%EC%9D%B4%EC%BD%98 (PageLinkReplacer.cs:45)
Failed to create page: Mask 텍스쳐 (206) statusCode:BadRequest (CfPage.cs:57)
Failed to create page: 아트/이펙트/텍스쳐/Mask conentType:html (CfSpace.cs:111)

### 240418

경로용 페이지 이름이 겹치는 문제, Milestone이라든지, `컴포넌트`라던지...

### 240416

success:173 failed:23
<mark class="marker-blue">UI 작업 완료</span>. - 157번 글.
42번 글.
<video controls="">

    <source type="video/webm" src="http://fileserver.bside.com:8081/WebLink/Movies/Tech/Milestone/202206/exception_editor.mkv">

    Sorry, your browser doesn't support embedded videos.
</video>
클라이언트 빌드 이슈 기록 (32) - 이건 그냥 본문이 너무 길어서 실패하는 게 아닐까 함. 

### 240414

success:172 failed:24

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