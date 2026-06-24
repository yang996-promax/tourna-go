import { Check, Minus } from 'lucide-react';

interface Props {
  checked: boolean;
  indeterminate?: boolean;
  onChange: () => void;
  className?: string;
}

export default function Checkbox({ checked, indeterminate, onChange, className = '' }: Props) {
  const active = checked || indeterminate;

  return (
    <button
      type="button"
      role="checkbox"
      aria-checked={indeterminate ? 'mixed' : checked}
      onClick={(e) => {
        e.stopPropagation();
        onChange();
      }}
      className={[
        'shrink-0 flex items-center justify-center w-5 h-5 rounded-md border-2 transition-all duration-200',
        'focus:outline-none focus-visible:ring-2 focus-visible:ring-amber-500/40 focus-visible:ring-offset-2 focus-visible:ring-offset-slate-950',
        active
          ? 'bg-amber-500 border-amber-400 text-slate-900 shadow-sm shadow-amber-500/30 scale-100'
          : 'bg-slate-800/60 border-slate-600 hover:border-amber-500/50 hover:bg-slate-800 active:scale-95',
        className,
      ].join(' ')}
    >
      {indeterminate && <Minus size={12} strokeWidth={3} />}
      {checked && !indeterminate && <Check size={12} strokeWidth={3} />}
    </button>
  );
}