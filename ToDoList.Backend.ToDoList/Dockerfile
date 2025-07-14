FROM postgres:15.4
# RUN groupadd -r postgres && useradd --no-log-init -r -g postgres postgres
ARG DB_LANG=ja_JP
RUN localedef -i $DB_LANG -c -f UTF-8 -A /usr/share/locale/locale.alias $DB_LANG.UTF-8
ENV LANG=$DB_LANG.utf8
USER postgres
