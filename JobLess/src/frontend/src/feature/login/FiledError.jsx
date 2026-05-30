
// FieldError.jsx — reusable error message component
// Used under every input to show validation feedback

export default function FieldError({ error }) {
  if (!error) return null;
  return <span className="field-error">{error}</span>;
}