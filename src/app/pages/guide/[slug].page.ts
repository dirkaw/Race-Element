import { Component,OnInit,ViewEncapsulation } from '@angular/core';
import { injectContent, MarkdownComponent } from '@analogjs/content';
import { AsyncPipe, CommonModule } from '@angular/common';
import PostAttributes from '../../post-attributes';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-guides-post',
  standalone: true,
  imports: [AsyncPipe, MarkdownComponent, CommonModule],
  template: `
    @if (post | async; as post) {
    <article class="rounded-lg container mx-auto max-w-4xl px-3">
      <h1 class="text-xl md:text-3xl font-['Conthrax'] select-none dark:text-gray-300 dark:bg-black rounded-tl-xl border-l-2 pl-2 pr-2 pt-1 pb-1 border-red-800">
        @if(post.attributes.type !== undefined){
          <a href="/guide">Guides</a> >
        }
         {{post.attributes.title}}
      </h1>
      <div class="container dark:bg-[#050505] pl-3 pr-[1em] pt-2 rounded-br-xl">
        <analog-markdown class="whitespace-pre-line" [content]="post.content" />
      </div>
    </article>
    }
  `,
  styles: [
    `
    `,
  ],
  encapsulation: ViewEncapsulation.None,
})
export default class GuideSlugComponent implements OnInit {
  readonly post = injectContent<PostAttributes>({
    param: 'slug',
    subdirectory: 'guide',
  });

  constructor(private meta: Meta) {}

  ngOnInit(): void {
    this.post.forEach(x => {
      this.meta.updateTag({ name: `og:url`, content: `https://race.elementfuture.com/guide/${x.attributes.slug}` })
      this.meta.updateTag({ name: `og:title`, content: `Race Element - Guide | ${x.attributes.title}` })
      this.meta.updateTag({ name: `og:description`, content: `${x.attributes.description}` })
      this.meta.updateTag({ name: `twitter:title`, content: `Race Element - Guide | ${x.attributes.title}` })
    });
  }
}
