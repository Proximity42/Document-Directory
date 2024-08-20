import {useEffect, useState} from 'react';
import { Input, Space, Button, DatePicker, Radio, Table, Select, Popover } from 'antd';
import { FolderFilled, FolderAddFilled, FileAddFilled, FileFilled, CloseOutlined, DeleteFilled, LeftOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
const { Search } = Input;
const { TextArea } = Input;

function MainPageComponent() {
    const [availableNodes, setAvailableNodes] = useState([]);
    const [hierarchy, setHierarchy] = useState('/');
    const [isDirectoryCreateFormVisible, setIsDirectoryCreateFormVisible] = useState(false);
    const [isDocumentCreateFormVisible, setIsDocumentCreateFormVisible] = useState(false);
    const [isShowInfoChosenDirectory, setIsShowInfoChosenDirectory] = useState(false);
    const [isShowInfoChosenDocument, setIsShowInfoChosenDocument] = useState(false);
    const [isDocumentViewModalVisible, setIsDocumentViewModalVisible] = useState(false);
    const [chosenNode, setChosenNode] = useState({})
    // const [isViewTable, setIsViewTable] = useState(false);
    // const [isViewIcons, setIsViewIcons] = useState(false);
    const [viewMethod, setViewMethod] = useState('table')
    const [authToken, setAuthToken] = useState("")
    const [directoryHierarchy, setDirectoryHierarchy] = useState([]);
    let documentFilterValue;

    async function searchDocumentByName(value, _e, info) {
        const response = await fetch('https://localhost:7018/api/documents/search', {
            
        })
    } 

    async function createDirectory() {
        const name = document.querySelector('#inputDirectoryName').value;
        const response = await fetch('https://localhost:7018/api/folders', {
            method: 'POST', 
            headers: new Headers({ "Content-Type": "application/json" }), 
            credentials: 'include',
            body: JSON.stringify({
                name: name,
                folderId: 0
            })
        });
        if (response.status == 201)
        {
            const json = await response.json();
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY')
            setAvailableNodes([...availableNodes, {...json, createdAt: createdAt}]);
            setIsDirectoryCreateFormVisible(false);
        }
    }

    async function createDocument() {
        const name = document.querySelector('#inputDocumentName').value;
        let outputDate = document.querySelector('#inputDocumentActivityDate').value;
        let activityDate = dayjs(outputDate, 'DD-MM-YYYY').format('YYYY-MM-DD');
        const content = document.querySelector('#inputDocumentContent').value;
        const date = dayjs(activityDate).toJSON();
        const response = await fetch('https://localhost:7018/api/documents', {
            method: 'POST', 
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
            body: JSON.stringify({
                name: name,
                content: content,
                activityEnd: date,
                folderId: 0
            })
        });
        if (response.status == 201)
        {
            const json = await response.json();
            const activityEnd = dayjs(json.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY')
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY')
            setAvailableNodes([...availableNodes, {...json, activityEnd: activityEnd, createdAt: createdAt}]);
            setIsDocumentCreateFormVisible(false);
        }
    }

    function onChangeDateActivity(date, dateString) {
    }

    async function deleteChosenNode() {
        const response = await fetch(`https://localhost:7018/api/documents/${chosenNode.id}`, {
            method: 'DELETE',
            headers: new Headers({"Content-Type": "application/json", 'Access-Control-Allow-Credentials': 'true'}),
            credentials: 'include',
        });
        if (response.status == 200)
        {
            setAvailableNodes(availableNodes.filter((node) => node.id != chosenNode.id));
            setChosenNode({});
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false);
        }
    }

    async function viewDirectoryContent(directory) {
        const response = await fetch(`https://localhost:7018/api/NodeHierarchy/${directory.id}`, {
            credentials: 'include',
            headers: new Headers({'Access-Control-Allow-Credentials': 'true'})
        });
        if (response.status == 200)
        {
            setAvailableNodes([]);
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false); 
        }
        return response.status;
    }

    async function viewNextDirectoryContent() {
        let status = await viewDirectoryContent(chosenNode);
        if (status == 200) {
            if (directoryHierarchy.length != 0) {
                setHierarchy((hierarchy) => hierarchy + '/');
            }
            setHierarchy((hierarchy) => hierarchy + chosenNode.name);
            setDirectoryHierarchy([...directoryHierarchy, chosenNode]);
            setChosenNode({});
        }
    }

    async function viewPrevDirectoryContent() {
        if (directoryHierarchy.length != 0)
        {
            let prevDirectory = directoryHierarchy.flat().slice(directoryHierarchy.length - 1)[0];
            const indexPrevDirectory = hierarchy.lastIndexOf(prevDirectory.name);
            setAvailableNodes([]);
            if (directoryHierarchy.length > 1)
            {
                let status = await viewDirectoryContent(prevDirectory);
                if (status == 200) {
                    setHierarchy((prevHierarchy) => prevHierarchy.substring(0, indexPrevDirectory));
                    setHierarchy((prevHierarchy) => prevHierarchy.substring(0, prevHierarchy.lastIndexOf('/')));
                    setDirectoryHierarchy(directoryHierarchy.filter((directory) => directory != prevDirectory));
                    setChosenNode({});
                }
            } else {
                await getAvailableNodes();
                setHierarchy((prevHierarchy) => prevHierarchy.substring(0, indexPrevDirectory));
                setDirectoryHierarchy([]);
                setChosenNode({});
            }
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false);
        }
        
    }

    async function getAvailableNodes() {
        const response = await fetch("https://localhost:7018/api/NodeHierarchy", {
            credentials: 'include',
            headers: new Headers({'Access-Control-Allow-Credentials': 'true'})
        });
        if (response.status == 200)
        {
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
        }
    }

    async function documentsFilter()
    {

    }

    const columns = [
        {
            title: 'Название',
            dataIndex: 'name',
            sorter: true,
            // render: (node) => `${node.name}`,
            width: '60%',
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
        }
    ];

    // const onChangeViewMethod = (value) => {
    //     if (value == "table")
    //     {
    //         setIsViewTable(true);
    //         setIsViewIcons(false);
            
    //     }
    //     else {
    //         setIsViewIcons(true);
    //         setIsViewTable(false);
    //     }
    // }

    useEffect(() => {
        getAvailableNodes();
    }, [])

    return (
        <>
            {isDirectoryCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название папки' id='inputDirectoryName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDirectoryCreateFormVisible(false)}/>
                        </div>
                        <Button size='small' onClick={createDirectory} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название документа' id='inputDocumentName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDocumentCreateFormVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата окончания действия документа</p>
                            <DatePicker id="inputDocumentActivityDate" onChange={onChangeDateActivity} format={{format: 'DD-MM-YYYY'}} placeholder='Выберите дату' placement='bottomLeft' minDate={dayjs()}/>
                        </div>
                        <TextArea
                            placeholder="Введите содержимое документа"
                            autoSize={{
                                minRows: 6,
                                maxRows: 10
                            }}
                            id="inputDocumentContent"
                        />
                        <Button size='small' onClick={createDocument} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentViewModalVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
                            <div>
                                <h2 style={{margin: '0', textWrap: 'wrap', maxWidth: '100%'}}>{chosenNode.name}</h2>
                            </div>
                            <CloseOutlined onClick={() => setIsDocumentViewModalVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата создания документа</p>
                            <DatePicker disabled format={{format: 'DD-MM-YYYY'}} placement='bottomLeft' defaultValue={dayjs(chosenNode.createdAt, "DD-MM-YYYY")} style={{color: 'black'}}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата окончания действия документа</p>
                            <DatePicker disabled format={{format: 'DD-MM-YYYY'}} placement='bottomLeft' defaultValue={dayjs(chosenNode.activityEnd, "DD-MM-YYYY")}/>
                        </div>
                        <TextArea disabled defaultValue={chosenNode.content} style={{textWrap: 'wrap', textAlign: 'left', opacity: '1', color: 'black', cursor: 'pointer'}} autoSize={{minRows: 18, maxRows: 20}}/>
                    </div>
                </div>
            )}
            <div>
                <Search
                    placeholder="Введите название документа"
                    allowClear
                    onSearch={searchDocumentByName}
                    style={{
                        maxWidth: '700px',
                        width: '70%'
                    }}
                />
                <div style={{display: 'flex', gap: '10px', marginTop: '15px'}}>
                    <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                        <p style={{ margin: '0', textAlign: 'left'}}>Фильтрация документов по дате</p>
                        <Radio.Group onChange={documentsFilter} value={documentFilterValue} style={{display: 'flex'}}>
                            <Radio value={1}>создания</Radio>
                            <Radio value={2}>активности</Radio>
                        </Radio.Group>
                    </div>
                    <div style={{display: 'flex', gap: '20px'}}>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>с</p>
                            <DatePicker placeholder="Укажите дату" style={{width: '130px'}}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>до</p>
                            <DatePicker placeholder="Укажите дату" style={{width: '130px'}}/>
                        </div>
                    </div>
                </div>
            </div>
            <div style={{display: 'flex', justifyContent: 'space-between', marginTop: '5px'}}>
                <div style={{display: 'flex', gap: '15px'}}>
                    <Popover content={<p style={{fontSize: '14px'}}>Создать папку</p>}>
                        <button onClick={() => setIsDirectoryCreateFormVisible(true)} className="btnWithIcon">
                            <FolderAddFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                    <Popover content={<p style={{fontSize: '14px'}}>Создать документ</p>}>
                        <button onClick={() => setIsDocumentCreateFormVisible(true)} className="btnWithIcon">
                            <FileAddFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                    <Popover content={<p style={{fontSize: '14px'}}>Удалить выбранный элемент</p>}>
                        <button onClick={() => deleteChosenNode()} className="btnWithIcon">
                            <DeleteFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                </div>
                {isShowInfoChosenDirectory && !isShowInfoChosenDocument && <div style={{fontSize: '14px', maxSize: '40%'}}>
                    <p>Имя папки: {chosenNode.name}</p>
                    <p>Дата создания: {chosenNode.createdAt}</p>
                </div>}
                {isShowInfoChosenDocument && !isShowInfoChosenDirectory && <div style={{fontSize: '14px', maxSize: '40%'}}>
                    <p>Имя документа: {chosenNode.name}</p>
                    <p>Дата создания: {chosenNode.createdAt}</p>
                    <p>Дата активности: {chosenNode.activityEnd}</p>
                </div>}
            </div>
            <div style={{display: 'flex', gap: '10px'}}>
                <button onClick={() => viewPrevDirectoryContent()} className="btnWithIcon">
                    <LeftOutlined style={{fontSize: '15px'}}/>
                </button>
                <p style={{textAlign: 'left', fontSize: '18px'}}>{hierarchy}</p>
            </div>
            <div style={{display: 'flex', alignItems: 'center', gap: '20px', marginBottom: '10px'}}>
                <p>Способ отображения</p>
                <Select onChange={setViewMethod} style={{width: '100px'}} defaultValue={'table'} options={[
                    { value: 'table', label: 'Таблица'},
                    { value: 'icons', label: 'Значки'}
                ]}/>
            </div>
            {viewMethod == 'icons' && <div>
                <Space wrap style={{gap: '15px'}}>
                    {availableNodes.map((node, index) => (
                        <div key={index} className='availableNode' style={{maxWidth: '120px'}}>
                            {node.type == "Directory" ? 
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDocument(false); setChosenNode(node); setIsShowInfoChosenDirectory(true);}} onDoubleClick={() => viewNextDirectoryContent()}>
                                <FolderFilled style={{fontSize: '50px'}} />
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p>
                            </button>
                            :
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDirectory(false); setChosenNode(node); setIsShowInfoChosenDocument(true);}} onDoubleClick={() => setIsDocumentViewModalVisible(true)}>
                                <FileFilled style={{fontSize: '45px'}}/>
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p> 
                            </button>
                            }
                        </div>
                    ))}
                </Space>
            </div>}
            {viewMethod == 'table' && <div>
                <Table 
                    columns={columns}
                    rowKey={(record) => record.id}
                    dataSource={availableNodes}
                />
            </div>}
            
        </>
    );
}

export default MainPageComponent;