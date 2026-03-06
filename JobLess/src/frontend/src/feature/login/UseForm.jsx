import { useState, useCallback } from "react";
import { validateField, validateForm } from "./UseFormValidation";

// ============================================================
// useForm hook — handles values, errors, touch, submit
// ============================================================

export default function useForm(initialValues, schema) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});

  // Update a single field value and re-validate it if already touched
  const handleChange = useCallback(
    (e) => {
      const { name, value } = e.target;
      setValues((prev) => {
        const next = { ...prev, [name]: value };
        if (touched[name]) {
          const error = validateField(name, value, next, schema);
          setErrors((prevErr) => ({ ...prevErr, [name]: error }));
        }
        return next;
      });
    },
    [touched, schema]
  );

  // Mark field as touched and validate on blur
  const handleBlur = useCallback(
    (e) => {
      const { name, value } = e.target;
      setTouched((prev) => ({ ...prev, [name]: true }));
      const error = validateField(name, value, values, schema);
      setErrors((prev) => ({ ...prev, [name]: error }));
    },
    [values, schema]
  );

  // Touch all fields and validate everything before submit
  const handleSubmit = useCallback(
    (onValid, onInvalid) => (e) => {
      e.preventDefault();
      const allTouched = Object.keys(schema).reduce((acc, k) => ({ ...acc, [k]: true }), {});
      setTouched(allTouched);
      const formErrors = validateForm(values, schema);
      setErrors(formErrors);
      if (Object.values(formErrors).some(Boolean)) {
        onInvalid?.(formErrors);
        return;
      }
      onValid(values);
    },
    [values, schema]
  );

  const getFieldProps = (name) => ({
    name,
    value: values[name] ?? "",
    onChange: handleChange,
    onBlur: handleBlur,
  });

  const getError = (name) => (touched[name] ? errors[name] : null);

  return {
    values,
    errors,
    touched,
    handleChange,
    handleBlur,
    handleSubmit,
    getFieldProps,
    getError,
    setValues,
  };
}