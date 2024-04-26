export default ctx => ({
  map: ctx.options.map,
  parser: ctx.options.parser,
  plugins: {
    tailwindcss: {},
    'postcss-preset-env': {
      env: ctx.env
    },
    cssnano: ctx.env === 'production' ? {} : false,
  }
})