﻿namespace WikiTool.Core.Transform;

using Html2Markdown.Replacement;
using Html2Markdown.Scheme;
using WikiTool.Core.Transform.CustomReplacer;

public sealed class ConvertingScheme : AbstractScheme
{
    public ConvertingScheme()
    {
        // remove '<a href="#개요" class="toc-anchor">¶</a>'
        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = @"<a href=""#.*?"" class=""toc-anchor"">¶</a>",
            Replacement = string.Empty,
        });

        this.ReplacerCollection.Add(new ImgTagReplacer());
        this.ReplacerCollection.Add(new CodeReplacer());
        this.ReplacerCollection.Add(new PreTagReplacer());
        this.ReplacerCollection.Add(new HeadingWithoutAttr());
        this.ReplacerCollection.Add(new PageLinkReplacer());
        this.ReplacerCollection.Add(new ListTagConverter());
        this.ReplacerCollection.Add(new TableTagConverter());
        this.ReplacerCollection.Add(new VideoTagConverter());

        // figure tag를 삭제.
        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "</?figure[^>]*>",
            Replacement = string.Empty,
        });

        // ‘<mark class="marker-yellow">Insert Assets</mark>’ -> ‘{color:yellow}Insert Assets{color}’
        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<mark class=\"marker-yellow\">",
            Replacement = "<span style=\"color: rgb(255,255,0);\">",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<mark class=\"pen-red\">",
            Replacement = "<span style=\"color: rgb(255,0,0);\">",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<mark class=\"marker-green\">",
            Replacement = "<span style=\"color: rgb(0,255,0);\">",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<mark class=\"marker-blue\">",
            Replacement = "<span style=\"color: rgb(0,0,255);\">",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<mark class=\"marker-pink\">",
            Replacement = "<span style=\"color: rgb(255, 192, 203);\">",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "</mark>",
            Replacement = "</span>",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<br>",
            Replacement = "<br />",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<hr>",
            Replacement = "<hr />",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "&nbsp;",
            Replacement = " ",
        });

        // <li><label class="todo-list__label"><input checked="checked" disabled="disabled" type="checkbox"><span class="todo-list__label__description"> sRGB 사용</span></label></li> -> <li> sRGB 사용</li>
        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "<li><label class=\"todo-list__label\"><input checked=\"checked\" disabled=\"disabled\" type=\"checkbox\"><span class=\"todo-list__label__description\">",
            Replacement = "<li>",
        });

        this.ReplacerCollection.Add(new PatternReplacer
        {
            Pattern = "</span></label></li>",
            Replacement = "</li>",
        });
    }
}
