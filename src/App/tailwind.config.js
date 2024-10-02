import defaultTheme from 'tailwindcss/defaultTheme'
import path from 'node:path'

import CatppuccinPlugin from '@catppuccin/tailwindcss'
import FormsPlugin from '@tailwindcss/forms'

const ProjectDirectory = process.env.MSBUILD_PROJECT_DIR
const SupportedFileExtensions = ['razor', 'razor.cs']

export default {
  content: SupportedFileExtensions.map(ext => path.join(ProjectDirectory, `./**/*.${ext}`)),
  plugins: [
    FormsPlugin(),
    CatppuccinPlugin({ prefix: 'ctp' })
  ],
  safelist: ['active', 'invalid', 'modified', 'validation-message'],
  theme: {
    extend: {
      backgroundImage: {
        'cs2-logo': "url('https://cdn.akamai.steamstatic.com/apps/csgo/images/csgo_react/global/logo_cs_sm.svg')"
      },
      fontFamily: {
        mono: ['IBM Plex Mono', ...defaultTheme.fontFamily.mono],
        sans: ['Nunito Sans', 'Noto Sans', 'Roboto', ...defaultTheme.fontFamily.sans]
      },
    }
  },
}