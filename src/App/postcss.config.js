export default ctx => ({
  map: ctx.options.map,
  parser: ctx.options.parser,
  plugins: {
    'postcss-prepend-imports': {
      path: './',
      files: ['_Imports.css']
    },
    '@tailwindcss/postcss': {},
    'postcss-preset-env': {
      autoprefixer: {},
      env: ctx.env
    },
    autoprefixer: {},
    cssnano: ctx.env === 'production' ? {} : false,
  }
})