import { Component,ViewEncapsulation } from '@angular/core';
import { injectContent, MarkdownComponent } from '@analogjs/content';
import { AsyncPipe, CommonModule } from '@angular/common';

import PostAttributes from '../../post-attributes';

@Component({
  selector: 'app-guides-post',
  standalone: true,
  imports: [AsyncPipe, MarkdownComponent, CommonModule],
  template: `
    @if (post | async; as post) {
    <article class="rounded-lg container mx-auto max-w-4xl">
      <h1 class="text-3xl font-['Conthrax'] select-none border-l-2 pl-2 border-red-800 dark:text-gray-300 dark:bg-black rounded-tl-xl border-l-2 pl-2 pr-2 pt-1 pb-1 border-red-800">
        @if(post.attributes.type !== undefined){
          <a href="/guide">Guides</a> >
        }
         {{post.attributes.title}}
      </h1>
      <div class="container dark:bg-[#050505] pl-3 pr-3 pt-2 rounded-br-xl">
        <p class="select-none">{{post.attributes.date !== undefined? (post.attributes.date | date:'longDate') : ''}}</p>
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
export default class GuideSlugComponent {
  readonly post = injectContent<PostAttributes>('slug');
}
