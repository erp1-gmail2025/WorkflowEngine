import { dirname } from "path";
import { fileURLToPath } from "url";
import { FlatCompat } from "@eslint/eslintrc";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const compat = new FlatCompat({
  baseDirectory: __dirname,
});

const eslintConfig = [
  ...compat.extends("next/core-web-vitals", "next/typescript"),
  {
    rules:{
      "@typescript-eslint/no-unused-vars": "off",
      "no-var": "off", // Allow use of 'var'
      "@typescript-eslint/no-explicit-any": "off", // Allow the use of 'any'
      "@typescript-eslint/no-implicit-any": "off", // Allow implicit 'any'
      "@typescript-eslint/no-empty-interface": "off", // Allow empty interfaces
      "@typescript-eslint/no-empty-object-type": "off", // Allow empty object types
      "import/no-anonymous-default-export": "off",
      "@typescript-eslint/no-use-before-define": "off", // Allow using code before its declaration
      "react/jsx-no-duplicate-props": "off", // Allow duplicate props (last one wins),
      "react/display-name": "off", // Allow components without display names
      "@typescript-eslint/no-unused-expressions": "off", // Allow unused expressions
    }
  }
];

export default eslintConfig;
