export function focusAndSelectEnd(element) {
  element.scrollIntoView({ behavior: "smooth" });
  element.focus();

  if (!!element.value) {
    element.setSelectionRange(element.value.length, element.value.length);
  }
}