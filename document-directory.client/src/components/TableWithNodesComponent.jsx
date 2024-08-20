

function TableWithNodesComponent({availableNodes}) {

    const columns = [
        {
            title: 'Название',
            dataIndex: 'name',
            sorter: true,
            width: '50%',
        },
        {
            title: 'Тип',
            dataIndex: 'type',
            render: (type) => type == "Directory" ? "Директория" : "Документ",
            filters: [
                {
                    text: 'Документ',
                    value: 'Document',
                },
                {
                    text: 'Директория',
                    value: 'Directory',
                },
            ],
            width: '10%',
        },
        {
            title: 'Дата создания',
            dataIndex: 'createdAt',
            width: '14%'
        },
        {
            title: 'Дата активности',
            dataIndex: 'activityEnd',
            width: '16%'
        },
        {
            title: <button>Редактировать</button>,
            width: '10%'
        }
    ];


    return (
        <Table 
            columns={columns}
            rowKey={(record) => record.id}
            dataSource={availableNodes}
        />
    )
}

export default TableWithNodesComponent;