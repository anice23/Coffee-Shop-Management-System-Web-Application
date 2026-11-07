
import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  OnDestroy,
  SimpleChanges,
} from '@angular/core';


@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './toast.html',
  styleUrl: './toast.css',
})
export class Toast implements OnChanges, OnDestroy {
  @Input() title = '';
  @Input() message = '';
  @Input() type: 'success' | 'error' = 'success';
  @Output() closed = new EventEmitter<void>();

  private timeoutId?: ReturnType<typeof setTimeout>;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['message'] && this.message) {
      this.clearTimer();

      this.timeoutId = setTimeout(() => {
        this.close();
      }, 4000);
    }
  }

  close(): void {
    this.clearTimer();
    this.message = '';
    this.closed.emit();
  }

  private clearTimer(): void {
    if (this.timeoutId !== undefined) {
      clearTimeout(this.timeoutId);
      this.timeoutId = undefined;
    }
  }

  ngOnDestroy(): void {
    this.clearTimer();
  }
}
