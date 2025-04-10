import { Component, HostBinding, Input } from '@angular/core';

@Component({
  selector: 'app-svg-icon',
  templateUrl: './svg-icon.component.html',
  styleUrls: ['./svg-icon.component.scss'],
})
export class SvgIconComponent {
  @HostBinding('style.mask-image')
  @HostBinding('style.webkitMaskImage')
  private _path!: string;

  @Input()
  set path(filePath: string) {
    this._path = `url("${filePath}")`;
  }

  @Input() size = '24px';
  @Input() label = 'icon';
  @Input() animation: 'pulse' | 'fade' | 'glow' | 'none' = 'none';

  @HostBinding('class') get animationClass(): string {
    return `svg-icon ${this.animation}`;
  }

  @HostBinding('attr.role') role = 'img';
  @HostBinding('attr.aria-label') get ariaLabel() {
    return this.label;
  }

  @HostBinding('style.height') get height() {
    return this.size;
  }

  @HostBinding('style.width') get width() {
    return this.size;
  }
}
